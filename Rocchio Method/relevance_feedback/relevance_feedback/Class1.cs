using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
//using ReadData;

namespace relevance_feedback
{
    class Program
    {


        public static Dictionary<T, int> SortByDuplicateCount<T>(IList<T> inputList)
        {
            //用于计算每个元素出现的次数，key是元素，value是出现次数
            Dictionary<T, int> distinctDict = new Dictionary<T, int>();
            for (int i = 0; i < inputList.Count; i++)
            {
                //这里没用trygetvalue，会计算两次hash
                if (distinctDict.ContainsKey(inputList[i]))
                    distinctDict[inputList[i]]++;
                else
                    distinctDict.Add(inputList[i], 1);
            }

            Dictionary<T, int> sortByValueDict = GetSortByValueDict(distinctDict);
            return sortByValueDict;
        }
        public static Dictionary<K, V> GetSortByValueDict<K, V>(IDictionary<K, V> distinctDict)
        {
            //用于给tempDict.Values排序的临时数组
            V[] tempSortList = new V[distinctDict.Count];
            distinctDict.Values.CopyTo(tempSortList, 0);
            Array.Sort(tempSortList); //给数据排序
            Array.Reverse(tempSortList);//反转

            //用于保存按value排序的字典
            Dictionary<K, V> sortByValueDict =
                new Dictionary<K, V>(distinctDict.Count);
            for (int i = 0; i < tempSortList.Length; i++)
            {
                foreach (KeyValuePair<K, V> pair in distinctDict)
                {
                    //比较两个泛型是否相当要用Equals，不能用==操作符
                    if (pair.Value.Equals(tempSortList[i]) && !sortByValueDict.ContainsKey(pair.Key))
                        sortByValueDict.Add(pair.Key, pair.Value);
                }
            }
            return sortByValueDict;
        }
        public sealed class ReverseComparer<T> : IComparer<T>
        {
            private readonly IComparer<T> inner;
            public ReverseComparer() : this(null) { }
            public ReverseComparer(IComparer<T> inner)
            {
                this.inner = inner ?? Comparer<T>.Default;
            }
            int IComparer<T>.Compare(T x, T y)
            {
                return inner.Compare(y, x);
            }
        }

        public static float VectorLength(int[] vector)
        {
            float sum = 0.0F;
            for (int i = 0; i < vector.Length; i++)
                sum = sum + (vector[i] * vector[i]);

            return (float)Math.Sqrt(sum);
        }

        static float InverseDocumentFrequency(int n_docnum, int n_strindocnum)
        {

            // N 文章總數
            // ni 字串出現的文章數

            return Convert.ToSingle(Math.Log(n_docnum / (n_strindocnum + 1), 2));
        }
        static float TermFrequencyWeights(float n_queryfreqnum)
        {
            //n_queryfreqnum 字串出現於此文章的次數
            return Convert.ToSingle(1.0 + Math.Log(n_queryfreqnum, 2));
        }

        public struct MyStruct
        {
            public string Filename;
            // public int[] FilenumberText;
            public float[] newweight;
            public float[] weight;
            public float[] sim;
            public float[] newsim;
            //public Dictionary<int, float> diction;
        }
        /* public  void similarity(MyStruct[] docst, float[] query)
                      {
                      
                              for (int i = 0; i < 2265; i++)
                              {
                               
                                  docst1.sim[i] = Convert.ToSingle(dot(docst[i].weight, query, 52000) / Math.Sqrt(dot(docst[i].weight, docst[i].weight, 52000) * dot(query, query, 52000)));
                              }
                       
                      }  */

        static float dot(float[] weight, float[] query, int n)
        {
            float sum = 0.0F;
            for (int i = 0; i < n; i++)
            {
                if (weight[i] > 0 && query[i] > 0)
                {
                    sum += (weight[i] * query[i]);
                }
            }
            return sum;
        }


      

        public static void PrintValues(String[] myArr, char mySeparator)
        {
            for (int i = 0; i < myArr.Length; i++)
                Console.Write("{0}{1}", mySeparator, myArr[i]);
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            //  string dataInfo = "c:\\SPLIT_DOC_WDID_NEW\\";
            DirectoryInfo dir = new DirectoryInfo("d:\\SPLIT_DOC_WDID_NEW\\");
            DirectoryInfo dir1 = new DirectoryInfo("d:\\QUERY_WDID_NEW\\");
            if (!dir.Exists)
            {
                Console.WriteLine("路徑不存在");
                Console.Read();
                return;
            }
            // Console.WriteLine("{0}資料夾下", dir.FullName);
            if (!dir1.Exists)
            {
                Console.WriteLine("路徑不存在");
                Console.Read();
                return;
            }
            // ReadData read = new ReadData(dataInfo);
            FileInfo[] doc = dir.GetFiles();
            FileInfo[] query = dir1.GetFiles();
            MyStruct[] docst = new MyStruct[2266];
            MyStruct[] queryst = new MyStruct[17];
            

            //FileInfo123 fii;
            StreamReader sr; //塞資料到temp
            StreamReader sr1; //塞資料到temp
            ArrayList temp = new ArrayList(500);
            ArrayList freqtemp = new ArrayList(500);
            Dictionary<int, double> docDict = new Dictionary<int, double>();
            ArrayList temp1 = new ArrayList(500);
            ArrayList freqtemp1 = new ArrayList(500);
            ArrayList arraytfidf = new ArrayList(500);

            string[] FilenumberText = new string[1000];
            string[] FilenumberText1 = new string[1000];
            int[] docni = new int[52000];
            float[] queryni = new float[52000];
            var docfreq = new Dictionary<int, float>();
            float docsum = 0;
            float[] querysum = new float[16];
            Dictionary<int, float> distinctDict = new Dictionary<int, float>();
            for (int a = 0; a < doc.Length; a++)
            {
                char[] delimiterChars = { ' ', '\n', '\r', };
                docst[a].Filename = doc[a].Name;

                sr = new StreamReader(doc[a].FullName);
                string FileText = "";

                //while (FileText != null)
                while ((FileText = sr.ReadLine()) != null)
                {


                    //FileText = sr.ReadLine();
                    if (FileText != null && !FileText.Equals(""))
                        FilenumberText = FileText.Split(delimiterChars);

                    //  LineList.Add(FilenumberText);
                    foreach (string s in FilenumberText)
                    {

                        if (s.Trim() != "")
                            if (s.Trim() != "-1")
                                temp.Add(s);

                    }
                }

                string[] FiletempnumberText = new string[temp.Count];
                temp.CopyTo(FiletempnumberText);
                temp.Clear();

                int[] FilenumberTextend = new int[FiletempnumberText.Length - 5];
                //  Dictionary<int, int> dicdocfreq = new Dictionary<int, int>();
                //docst[a].FilenumberText = new int[FiletempnumberText.Length - 5];
                Dictionary<char, int> dictdocfreq = new Dictionary<char, int>();
                docst[a].weight = new float[52000];
                docst[a].newweight = new float[52000];

                /*    List<int> allWords = new List<int>();
                    for (int i = 0; i < FiletempnumberText.Length - 5; i++)
                    {
                    
                        FilenumberTextend[i] = Convert.ToInt32(FiletempnumberText[i + 5]);
                        allWords.Add(FilenumberTextend[i]);
                   
                    }
                 
               
    
                var tempdocfreq=from t in FilenumberTextend
                 group t by t into m
                 select new {m.Key,count=m.Count()};
              //  docst[a].diction = new Dictionary<int, double>();
                docst[a].diction = tempdocfreq.ToDictionary(q => q.Key, q => TermFrequencyWeights(q.count));*/
                // Dictionary<int, double> dic_docfreq = tempdocfreq.ToDictionary(q => q.Key, q =>TermFrequencyWeights( q.count));
                /* foreach(var d1 in dd)
                 {
                  Console.WriteLine(string.Format("Key= {0}   value={1}",d1.Key,d1.Value));
                 }*/

                for (int i = 0; i < FiletempnumberText.Length - 5; i++)
                {
                    int n_tempNumber = 0;
                    FilenumberTextend[i] = Convert.ToInt32(FiletempnumberText[i + 5]);

                    n_tempNumber = FilenumberTextend[i];


                    docst[a].weight[n_tempNumber]++;

                }

                sr.Close();
            }


            for (int a = 0; a < doc.Length; a++)
            {
                for (int j = 0; j < 52000; j++) //算 ni 
                {

                    if (docst[a].weight[j] > 0)
                    {

                        docni[j]++;

                    }

                }
            }

            for (int a = 0; a < doc.Length; a++)
            {
                for (int j = 0; j < 52000; j++) //算doc  tfidf
                {

                    if (docst[a].weight[j] > 0)
                    {

                        // Console.WriteLine("docni第{0}個的number為{1}", j, docni[j]);
                        docst[a].weight[j] = TermFrequencyWeights(docst[a].weight[j]);
                        
                        docst[a].weight[j] = docst[a].weight[j] * InverseDocumentFrequency(doc.Length, docni[j]);
                        docst[a].newweight[j] = docst[a].weight[j];
                        // docsum += docst[a].weight[j] * docst[a].weight[j];
                    }

                }
            }






            for (int b = 0; b < query.Length; b++)//讀檔
            {
                char[] delimiterChars1 = { ' ', '\n', '\r', };
                queryst[b].Filename = query[b].Name;
                sr1 = new StreamReader(query[b].FullName);
                string FileText1 = "";


                while (FileText1 != null)
                {


                    FileText1 = sr1.ReadLine();
                    if (FileText1 != null && !FileText1.Equals(""))
                        FilenumberText1 = FileText1.Split(delimiterChars1);

                    //  LineList.Add(FilenumberText);
                    foreach (string s1 in FilenumberText1)
                    {

                        if (s1.Trim() != "")
                            if (s1.Trim() != "-1")
                                temp1.Add(s1);

                    }
                }

                string[] FiletempnumberText1 = new string[temp1.Count];
                temp1.CopyTo(FiletempnumberText1);
                temp1.Clear();

                int[] FilenumberTextend1 = new int[FiletempnumberText1.Length];
                // queryst[b].FilenumberText = new int[FiletempnumberText1.Length];
                queryst[b].weight = new float[52000];
                queryst[b].newweight = new float[52000];

                for (int i = 0; i < FiletempnumberText1.Length; i++)
                {
                    int n_tempNumber1 = 0;
                    FilenumberTextend1[i] = Convert.ToInt32(FiletempnumberText1[i]);
                    // queryst[b].FilenumberText[i] = Convert.ToInt32(FilenumberTextend1[i]);
                    n_tempNumber1 = FilenumberTextend1[i];
                    queryst[b].weight[n_tempNumber1]++;

                }
                sr1.Close();


            }
            // queryst[b].weight = new double[52000];
            for (int b = 0; b < query.Length; b++)
            {
                for (int j = 0; j < 52000; j++) //算query ni tfidf
                {
                    querysum[b] = 0.0F;
                    if (queryst[b].weight[j] > 0)
                    {


                        // queryni[j]++;

                        queryst[b].weight[j] = TermFrequencyWeights(queryst[b].weight[j]);
                        //Console.WriteLine("第{0}個query的第{1}number為{2}", b, j, queryst[b].weight[j]);
                         
                        queryst[b].weight[j] = queryst[b].weight[j] * InverseDocumentFrequency(doc.Length, docni[j]);
                       queryst[b].newweight[j] = queryst[b].weight[j];

                    }


                }
            }
             for (int b = 0; b < query.Length; b++)//sim
             {
                 queryst[b].sim = new float[doc.Length];
                 for (int a = 0; a < doc.Length; a++)
                 {
                     queryst[b].sim[a] = Convert.ToSingle(dot(docst[a].weight, queryst[b].weight, 52000) / (Math.Sqrt(dot(docst[a].weight, docst[a].weight, 52000)) * Math.Sqrt(dot(queryst[b].weight, queryst[b].weight, 52000))));
                     // similarity(docst[a], queryst);

                 }
             }
            for (int a = 0; a < 16; a++)
            {
                // StreamWriter sw = new StreamWriter("c:\\123456.txt");
                int Rocchionum = 3;
                float releventB = 0.8F;
                float unreleventB = 0.1F;
                float[] newweight = new float[52000];

                float[] newweight1 = new float[52000];
                //Dictionary<string, float> openWith = new Dictionary<string, float>();
                Dictionary<int, float> openWith1 = new Dictionary<int, float>();
                for (int b = 0; b < doc.Length; b++)
                {
                    // openWith.Add(queryst[a].sim[b], b);
                    //openWith.Add(doc[b].Name, queryst[a].sim[b]);
                    openWith1.Add(b, queryst[a].sim[b]);
                    // sd.Add(queryst[a].sim[b], doc[b].Name);
                }
                //var sortdic = from d in openWith orderby d.Value descending select d;
                var sortdic = from d in openWith1 orderby d.Value descending select d;
                //Dictionary<string, float> dic2 = sortdic.ToDictionary(c => c.Key, c => c.Value);
                Dictionary<int, float> dic21 = sortdic.ToDictionary(c => c.Key, c => c.Value);
                //openWith.Reverse();
                /*foreach (KeyValuePair<double, string> item in sd.Reverse())
                {
                    dc.Add(item.Key, item.Value);
                }
                sd.Clear();
                foreach (KeyValuePair<double, string> item in dc)
                {
                    sw.WriteLine("{0}    {1}", item.Value,item.Key );
                }*/
                int[] te = new int[dic21.Count];
                dic21.Keys.CopyTo(te, 0);

                openWith1.Clear();
                dic21.Clear();
                
                for (int i = 0; i < Rocchionum; i++)
                {

                    for (int j = 0; j < 52000; j++)
                    {

                         if (docst[te[i]].newweight[j] > 0)
                         {
                        newweight[j] += (docst[te[i]].newweight[j]);//找前k個doc tfidf
                        // queryst[a].newweight[j] += (docst[te[i]].newweight[j]);
                       
                               // r_Rocchiosum+= (docst[te[i]].newweight[j]*docst[te[i]].newweight[j]);
                            }
                        }

                    }

                
               /* for (int i = 0; i < Rocchionum; i++)
                {

                    for (int j = 0; j < 52000; j++)
                    {
                        //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                       // newweight[j] = (releventB * queryst[a].newweight[j] )/ Rocchionum;
                       // newweight[j] = (releventB * queryst[a].newweight[j] )/ Rocchionum;
                        //newweight[j] = (releventB * (queryst[a].newweight[j] / r_Rocchiosum)) / Rocchionum; ;
                    }
                }*/

                for (int j = 0; j < 52000; j++)
                {
                    //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                    // newweight[j] = (releventB * queryst[a].newweight[j] )/ Rocchionum;
                    newweight[j] = (releventB * newweight[j]) / Rocchionum;
                    //newweight[j] = (releventB * (queryst[a].newweight[j] / r_Rocchiosum)) / Rocchionum; ;
                }
                for (int i = Rocchionum; i < doc.Length; i++)
                {

                    for (int j = 0; j < 52000; j++)
                    {

                        newweight1[j] += (docst[te[i]].newweight[j]);
                        // queryst[a].newweight[j] += (docst[te[i]].newweight[j]);

                    }

                }
               /* for (int i = Rocchionum; i < doc.Length; i++)
                {

                    for (int j = 0; j < 52000; j++)
                    {
                        //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                        //newweight1[j] = (unreleventB * queryst[a].newweight[j]) / (doc.Length - Rocchionum);
                        newweight1[j] = (unreleventB * queryst[a].newweight[j]) /52000;
                    }
                }*/
                for (int j = 0; j < 52000; j++)
                {
                    //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                    //newweight1[j] = (unreleventB * queryst[a].newweight[j]) / (doc.Length - Rocchionum);
                    newweight1[j] = (unreleventB * newweight1[j]) / (doc.Length - Rocchionum);
                }

                /*for (int i = 0; i < doc.Length; i++)
                {

                    for (int j = 0; j < 52000; j++)
                    {
                        //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                        queryst[a].newweight[j] = queryst[a].weight[j] + newweight[j] - newweight1[j];
                        //queryst[a].newweight[j] = queryst[a].newweight[j] * InverseDocumentFrequency(doc.Length, docni[j]);
                    }
                }*/
                for (int j = 0; j < 52000; j++)
                {
                    //queryst[a].newweight[j] = releventB * queryst[a].newweight[j] / Rocchionum;
                    queryst[a].newweight[j] = queryst[a].weight[j] + newweight[j] - newweight1[j];
                    //queryst[a].newweight[j] = queryst[a].newweight[j] * InverseDocumentFrequency(doc.Length, docni[j]);
                }
                // dc.Clear();
                newweight.Initialize();
                newweight1.Initialize();




                //sw.Dispose();
            }

            for (int b = 0; b < query.Length; b++)//新sim
            {
                queryst[b].newsim = new float[doc.Length];
                for (int a = 0; a < doc.Length; a++)
                {
                    queryst[b].newsim[a] = Convert.ToSingle(dot(docst[a].newweight, queryst[b].newweight, 52000) / (Math.Sqrt(dot(docst[a].newweight, docst[a].newweight, 52000)) * Math.Sqrt(dot(queryst[b].newweight, queryst[b].newweight, 52000))));
                    // similarity(docst[a], queryst);

                }
            }


            ArrayList a_tempsum = new ArrayList(200);


            SortedDictionary<float, string> sd = new SortedDictionary<float, string>();
            Dictionary<float, string> dc = new Dictionary<float, string>();
            //SortedDictionary<double, string> openWith = new SortedDictionary<double, string>(new IntegerDecreaseComparer());
            //StreamWriter sw = new StreamWriter(@"c:\\123456.txt");
            /* SortedDictionary<string, object> dictionary = blah;
             var keyValuePairs = dictionary.Reverse();
             var keys = dictionary.Keys.Reverse();*/

            //var openWith = new SortedDictionary<double, string>(new ReverseComparer<double>());
            // SortedDictionary<string, float> openWith = new SortedDictionary<string, float>(new ReverseComparer<float>());
            // for (int a = 0; a < query.Length; a++)
            StreamWriter sw = new StreamWriter(@"d:\\ResultsTrainSetrelevent.txt");//寫檔
            for (int a = 0; a < 16; a++)
            {
                // StreamWriter sw = new StreamWriter("c:\\123456.txt");

                sw.WriteLine("query {0}    {1}  2265 ", a + 1, queryst[a].Filename);
                Dictionary<string, float> openWith = new Dictionary<string, float>();
                //Dictionary<int, float> openWith1 = new Dictionary<int, float>();
                for (int b = 0; b < doc.Length; b++)
                {
                    // openWith.Add(queryst[a].sim[b], b);
                    openWith.Add(doc[b].Name, queryst[a].newsim[b]);
                    // openWith1.Add(b, queryst[a].sim[b]);
                    // sd.Add(queryst[a].sim[b], doc[b].Name);
                }
                //var sortdic = from d in openWith orderby d.Value descending select d;
                var sortdic = from d in openWith orderby d.Value descending select d;
                Dictionary<string, float> dic2 = sortdic.ToDictionary(c => c.Key, c => c.Value);
                // Dictionary<int, float> dic21 = sortdic.ToDictionary(c => c.Key, c => c.Value);
                //openWith.Reverse();
                /*foreach (KeyValuePair<double, string> item in sd.Reverse())
                {
                    dc.Add(item.Key, item.Value);
                }
                sd.Clear();
                foreach (KeyValuePair<double, string> item in dc)
                {
                    sw.WriteLine("{0}    {1}", item.Value,item.Key );
                }*/



                foreach (var entry in dic2)
                {
                    sw.WriteLine("{0}    {1}", entry.Key, entry.Value);
                }
                // dc.Clear();

                sw.Flush();
                openWith.Clear();

                sw.WriteLine("");
                dic2.Clear();
                //sw.Dispose();
            }




            //Console.Write("sd");
            // Console.Read();
        }

    }
}





