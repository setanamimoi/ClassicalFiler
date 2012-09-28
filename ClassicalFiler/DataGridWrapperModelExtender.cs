using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Windows.Controls;

namespace ClassicalFiler
{
    /// <summary>
    /// <![CDATA[
    /// DataGrid のItemSource のプロパティで読み取り専用プロパティを変更する場合に
    /// 動的クラスをモデルのラッパーオブジェクトとして管理するクラスです。
    /// ]]>
    /// </summary>
    /// <typeparam name="T">管理する ItemSouce のラッパー元のクラス</typeparam>
    public class DataGridWrapperModelExtender<T> : DataGridExtender where T : class
    {
        /// <summary>
        /// DataGridDynamicModelBinder クラスのデフォルトコンストラクタによるインスタンス初期化は許可していません。
        /// </summary>
        protected DataGridWrapperModelExtender() : base()
        {
        }
        /// <summary>
        /// <![CDATA[
        /// 管理対象の DataGrid のインスタンスを指定して
        /// DataGridDynamicModelBinder クラスの新しいインスタンスを初期化します。
        /// ]]>
        /// </summary>
        /// <param name="source">管理対象のDataGrid</param>
        public DataGridWrapperModelExtender(DataGrid source) : base(source)
        {
        }

        /// <summary>
        /// 選択中の DataContext のオブジェクト を取得します。
        /// </summary>
        public T SelectedDataContext
        {
            get
            {
                dynamic wrapItem = this.Source.SelectedItem;
                if (wrapItem == null)
                {
                    return null;
                }

                return wrapItem.Instance as T;
            }
            set
            {
                if (value == null)
                {
                    this.Source.SelectedItem = null;
                    return;
                }

                foreach (dynamic wrapItem in this.Source.Items)
                {
                    if (value.Equals(wrapItem.Instance) == true)
                    {
                        this.Source.SelectedItem = wrapItem;
                        return;
                    }
                }

                this.Source.SelectedItem = CreateWrapModel(value).FirstOrDefault();
            }
        }

        /// <summary>
        /// 選択中の DataContext のオブジェクト を取得します。
        /// </summary>
        public T[] SelectedDataContexts
        {
            get
            {
                List<T> ret = new List<T>();

                dynamic[] wrapItems = this.Source.SelectedItems.Cast<dynamic>().ToArray() as dynamic[];
                if (wrapItems == null)
                {
                    return ret.ToArray();
                }
                foreach (dynamic wrapItem in wrapItems)
                {
                    T item = wrapItem.Instance as T;
                    if (item != null)
                    {
                        ret.Add(item);
                    }
                }

                return ret.ToArray();
            }
            set
            {
                if (value == null)
                {
                    this.Source.SelectedItems.Clear();
                    return;
                }
                
                foreach (dynamic wrapItem in this.Source.Items)
                {
                    T item = wrapItem.Instance as T;
                    if (value.Contains(item) == true)
                    {
                        this.Source.SelectedItems.Add(wrapItem);
                    }
                }
            }
        }

        /// <summary>
        /// DataContext となるオブジェクトの配列を取得・設定します。
        /// </summary>
        public T[] ItemsSouce
        {
            get
            {
                List<T> ret = new List<T>();
                foreach (dynamic d in this.Source.Items)
                {
                    ret.Add(d.Instance);
                }
                return ret.ToArray();
            }
            set
            {
                dynamic[] bindModels = CreateWrapModel(value);

                this.Source.ItemsSource = bindModels;
            }
        }
        
        /// <summary>
        /// 指定した DataGridRow の DataContext を取得します。
        /// </summary>
        /// <param name="row">DataContext を取得する元の DataGridRow</param>
        /// <returns>取得した DataContext</returns>
        public T GetDataContext(DataGridRow row)
        {
            dynamic element = row.DataContext as ExpandoObject;
            if (element == null)
            {
                return null;
            }

            T elementPath = element.Instance as T;

            if (elementPath == null)
            {
                return default(T);
            }

            return elementPath;
        }

        /// <summary>
        /// 指定した DataGridRow に 指定した DataContext をセットします。
        /// </summary>
        /// <param name="row">DataContext を設定する先の DataGridRow</param>
        /// <param name="dataContext">設定する DataContext</param>
        public void SetDataContext(DataGridRow row, T dataContext)
        {
            dynamic element = row.DataContext as ExpandoObject;
            element.Instance = dataContext;
        }

        /// <summary>
        /// ラッピングする元のオブジェクト配列を指定して読み取り・書き込み可能なプロパティを保持する ラッパーモデルを作成します。
        /// </summary>
        /// <param name="models">ラッパー元モデル</param>
        /// <returns>ラッパーモデルの配列</returns>
        public static dynamic[] CreateWrapModel(params T[] models)
        {
            List<object> ret = new List<object>();

            foreach (T dataContext in models)
            {
                dynamic wrapModel = new ExpandoObject();

                PropertyInfo[] properties = dataContext.GetType().GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    object propertyValue = property.GetValue(dataContext, null);

                    IDictionary<string, object> wrapModelDictionary = wrapModel as IDictionary<string, object>;

                    wrapModelDictionary.Add(property.Name, propertyValue);
                }

                wrapModel.Instance = dataContext;
                ret.Add(wrapModel);
            }

            return ret.ToArray();
        }
    }
}
