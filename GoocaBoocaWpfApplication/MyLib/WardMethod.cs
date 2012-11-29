using System;
using System.Net;
using System.Collections.Generic;
using System.Linq;

namespace MyLib.Analyze
{
    public class WardMethod : System.ComponentModel.BackgroundWorker
    {
        public WardMethod()
            : base()
        {
            this.WorkerReportsProgress = true;
            this.DoWork += new System.ComponentModel.DoWorkEventHandler(WardMethod_DoWork);

        }

        public List<Cluster> GetCluster(int num)
        {
            if (clusterList.Any())
            {
                var maxLayer = clusterList.First().Layer;
                var top = clusterList.First();
                for (int i = maxLayer; i > 0; i--)
                {
                    var list = top.GetCluster(i);
                    if (list.Count >= num)
                    {
                        return list;
                    }
                }
                return top.GetCluster(0);
            }
            else
            {
                return new List<Cluster>();
            }
        }

        private NormalizeType NormalizeType = NormalizeType.比;

        private void Normalize()
        {
            if (NormalizeType == Analyze.NormalizeType.比)
            {
                double[] sum = new double[inputDataList.Min(n => n.Value.Length)];
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i] = 0;
                }
                foreach (var item in inputDataList)
                {
                    for (int i = 0; i < sum.Length; i++)
                    {
                        sum[i] += Math.Pow(item.Value[i], 2);
                    }
                }
                for (int i = 0; i < sum.Length; i++)
                {
                    sum[i] = sum[i] / inputDataList.Count;
                }
                foreach (var item in inputDataList)
                {
                    for (int i = 0; i < sum.Length; i++)
                    {
                        item.Value[i] = Math.Sqrt(Math.Pow(item.Value[i], 2) / sum[i]);
                    }
                }
            }

        }


        void WardMethod_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            clusterList.Clear();
            Normalize();
            foreach (var item in inputDataList)
            {
                Cluster c = new Cluster();
                c.Datas.Add(item);
                c.Layer = 0;

                clusterList.Add(c);
            }
            int layer = 1;
            while (clusterList.Count > 1)
            {
                List<Pair> pairList = new List<Pair>();
                int i = 1;
                foreach (var item in clusterList.Take(clusterList.Count() - 1))
                {
                    foreach (var item2 in clusterList.Skip(i))
                    {
                        var pair = new Pair()
                        {
                            ClusterA = item2,
                            ClusterB = item
                        };
                        pairList.Add(pair);
                    }
                    i++;
                }
                System.Threading.Tasks.Parallel.ForEach(pairList, (n) =>
                {
                    n.Value = n.ClusterA.GetWardValue(n.ClusterB);
                });

                var min = pairList.Min(n => n.Value);
                List<List<Cluster>> cList = new List<List<Cluster>>();
                foreach (var item in pairList.Where(n => n.Value == min))
                {
                    bool flag = true;
                    foreach (var item2 in cList)
                    {
                        if (item2.Any(n => n == item.ClusterA || n == item.ClusterB))
                        {
                            item2.Add(item.ClusterA);
                            item2.Add(item.ClusterB);
                            flag = false;
                            break;
                        }
                    }
                    if (flag)
                    {
                        cList.Add(new List<Cluster> { item.ClusterA, item.ClusterB });
                    }
                }

                foreach (var item in cList)
                {
                    var nCluster = new Cluster() { Layer = layer };
                    foreach (var item2 in item.Distinct())
                    {
                        clusterList.Remove(item2);
                        nCluster.AddChild(item2);
                    }
                    nCluster.SetDatas();
                    clusterList.Add(nCluster);

                }
                layer++;
                ReportProgress(100 - clusterList.Count * 100 / inputDataList.Count);
            }
            ReportProgress(100);

        }
        private List<Cluster> clusterList = new List<Cluster>();

        public List<Cluster> ClusterList
        {
            get { return clusterList; }
            set { clusterList = value; }
        }

        private List<Data> inputDataList = new List<Data>();
        public void AddData(Data data)
        {
            inputDataList.Add(data);
        }
        public void DataClear()
        {
            inputDataList.Clear();
        }

        public void Run()
        {
            this.RunWorkerAsync();
        }

        public class Pair
        {
            public Cluster ClusterA { get; set; }
            public Cluster ClusterB { get; set; }
            public double Value { get; set; }
        }

        public class Cluster
        {
            public Cluster Parent { get; set; }
            List<Cluster> children = new List<Cluster>();

            public List<Cluster> Children
            {
                get { return children; }
                set { children = value; }
            }
            public int Layer { get; set; }
            public object Tag { get; set; }
            List<Data> datalist = new List<Data>();

            public List<Data> Datas
            {
                get { return datalist; }
                set { datalist = value; }
            }
            public Data CenterData { get; protected set; }
            public void SetDatas()
            {
                List<Data> list = new List<Data>();
                foreach (var item in Children)
                {
                    list.AddRange(item.Datas);
                }
                Datas = list;
                err = GetErr();
            }

            private Dictionary<Cluster, double> pairValueDic = new Dictionary<Cluster, double>();
            private object obj = new object();

            public double GetWardValue(Cluster c)
            {
                if (pairValueDic.ContainsKey(c))
                {
                    return pairValueDic[c];
                }
                else
                {
                    var v = WardDistance(c);
                    lock (obj)
                    {
                        pairValueDic.Add(c, v);
                    }
                    return v;
                }
            }

            public void ClearPairValueDic()
            {
                pairValueDic.Clear();
            }

            public void AddChild(Cluster c)
            {
                children.Add(c);
                c.Parent = this;
                c.ClearPairValueDic();
                //  SetDatas();
            }
            public void SetCenter()
            {
                CenterData = Data.GetCenter(Datas);
            }

            public double WardDistance(Cluster c)
            {
                List<Data> list = new List<Data>(this.Datas);
                list.AddRange(c.Datas);
                return GetErr(list) - this.Err - c.Err;
            }
            double err = -1;
            public double Err
            {
                get
                {
                    if (err < 0)
                    {
                        err = GetErr();
                    }
                    return err;
                }
            }

            private double GetErr()
            {
                if (CenterData == null)
                {
                    SetCenter();
                }
                double sum = 0;
                foreach (var item in Datas)
                {
                    sum += CenterData.Distance(item);
                }
                return sum;
            }
            private double GetErr(List<Data> list)
            {
                var c = Data.GetCenter(list);
                double sum = 0;
                foreach (var item in list)
                {
                    sum += c.Distance(item);
                }
                return sum;
            }

            public List<Cluster> GetCluster(int layer)
            {
                List<Cluster> list = new List<Cluster>();
                if (this.Layer > layer)
                {
                    foreach (var item in Children)
                    {
                        list.AddRange(item.GetCluster(layer));
                    }
                }
                else
                {
                    list.Add(this);
                }
                return list;
            }
        }

        public class Data
        {
            public double[] Value { get; set; }
            public double[] OriginValue { get; set; }
            public object Tag { get; set; }
            public Data() { }
            public Data(double[] value)
            {
                this.Value = value;
                this.OriginValue = value;
            }


            public virtual double Distance(Data d)
            {
                return MyLib.MathLib.GetDistance(this.Value, d.Value);
            }

            public static Data GetCenter(List<Data> list)
            {
                var min = list.Min(n => n.Value.Length);
                double[] center = new double[min];
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = 0;
                }

                foreach (var item in list)
                {
                    for (int i = 0; i < center.Length; i++)
                    {
                        center[i] += item.Value[i];
                    }
                }
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = center[i] / list.Count;
                }
                return new Data() { Value = center };

            }

            public static double[] GetOriginCenter(List<Data> list)
            {
                var min = list.Min(n => n.Value.Length);
                double[] center = new double[min];
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = 0;
                }

                foreach (var item in list)
                {
                    for (int i = 0; i < center.Length; i++)
                    {
                        center[i] += item.OriginValue[i];
                    }
                }
                for (int i = 0; i < center.Length; i++)
                {
                    center[i] = center[i] / list.Count;
                }
                return  center ;
            }
        }
    }
    public enum NormalizeType
    {
        平均分散, 比
    }
}
