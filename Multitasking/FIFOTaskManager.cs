using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    class FIFOTaskManager : ITaskManager
    {
        protected int[] X = new int[10];
        protected int[] K = new int[10];
        protected int[] V = new int[10];
        protected int[] H = new int[10];
        protected int[] T = new int[10];
        protected int[] Tp = new int[10];
        protected int[] Tz = new int[10];

        protected int[] realTp = new int[10];
        protected int[] realStartT = new int[10];
        protected int[] realEndT = new int[10];
        protected int[] activeT = new int[10];
        protected double[] Wi = new double[10];
        public double averageW;

        protected int zeroX;
        protected int overallTime;
        protected bool allDone = false;

        //default values
        protected int systemV = 16;
        protected int systemH = 12;
        protected int[] defaultV = { 6, 3, 2, 4, 3, 5, 7, 9, 4, 1 };
        protected int[] defaultH = { 2, 4, 3, 1, 2, 0, 4, 1, 6, 3 };
        protected int[] defaultT = { 70, 90, 40, 10, 30, 30, 20, 30, 40, 50 };

        //Process management
        protected List<Process> processList = new List<Process>();
        protected List<Process> runningList = new List<Process>();
        protected List<Process> waitingList = new List<Process>();
        protected List<Process> loadingList = new List<Process>();
        protected List<Process> completedList = new List<Process>();

        //statistic table
        protected List<int> ststisticList = new List<int>();
        protected List<int> remainingV = new List<int>();
        protected List<int> remainingH = new List<int>();
        protected List<int> multiProcessState = new List<int>();
        protected Dictionary<int, List<string>> multiTaskDictionary = new Dictionary<int, List<string>>();

        public FIFOTaskManager(int zeroX)
        {
            this.zeroX = zeroX;
            CalculateX();
            CalculateK();
            CalculateTp();
            CalculateVHTTz();
            FillMultiTaskDictionary();
            remainingV.Add(systemV);
            remainingH.Add(systemH);
            multiProcessState.Add(0);
            while (!allDone) { ExecuteTasks(); }
            completedList.Sort();
        }

        public int[] GetX() { return X; }
        
        public int[] GetK() { return K; }

        public int[] GetTp() { return Tp; }

        public int[] GetV() { return V; }

        public int[] GetH() { return H; }

        public int[] GetT() { return T; }

        public int[] GetTz() { return Tz; }

        public int[] GetRealTp()
        {
            for (int i = 0; i < 10; i++)
            {
                realTp[i] = completedList[i].GetRealTp();
            }
            return realTp;
        }

        public int[] GetRealStartT()
        {
            for (int i = 0; i < 10; i++)
            {
                realStartT[i] = completedList[i].GetRealStartT();
            }
            return realStartT;
        }

        public int[] GetRealEndT()
        {
            for (int i = 0; i < 10; i++)
            {
                realEndT[i] = completedList[i].GetRealEndT();
            }
            return realEndT;
        }

        public int[] GetActiveT()
        {
            for (int i = 0; i < 10; i++)
            {
                activeT[i] = completedList[i].GetActiveTime();
            }
            return activeT;
        }

        public double[] GetWi()
        {
            for (int i = 0; i < 10; i++)
            {
                Wi[i] = completedList[i].GetWi();
            }
            averageW = Math.Round((Wi.Average()), 3);
            return Wi;
        }


        public List<int> GetVHistory() { return remainingV; }

        public List<int> GetHHistory() { return remainingH; }

        public Dictionary<int, List<string>> GetDictionary() { return multiTaskDictionary; }

        public bool GetAllDone() { return allDone; }

        public List<int> GetMultiprocState() { return multiProcessState; }


        public void CalculateX()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    int firstX = (7 * zeroX + 417) % 1000;
                    X[i] = firstX;
                }
                else
                {
                    X[i] = (7 * X[i - 1] + 417) % 1000;
                }
            }
        }

        public void CalculateK()
        {
            for (int i = 0; i < 10; i++)
            {
                double temp = Convert.ToDouble(X[i]) / Convert.ToDouble(7);
                double temp2 = Math.Round(temp, MidpointRounding.ToEven);
                int temp3 = Convert.ToInt32(temp2);
                K[i] = temp3 % 10;   
            }
        }

        public void CalculateTp()
        {
            for (int i = 0; i < 10; i++)
            {
                if (i == 0)
                {
                    Tp[i] = 0;
                }
                else if (i == 1)
                {
                    Tp[i] = K[0] + K[1];
                }
                else
                {
                    Tp[i] = Tp[i - 1] + K[i];
                }
            }
        }

        public void CalculateVHTTz()
        {
            for (int i = 0; i < 10; i++)
            {
                Process process = new Process(i);

                V[i] = defaultV[K[i]];
                H[i] = defaultH[K[i]];
                T[i] = defaultT[K[i]];
                Tz[i] = H[i] * 5;

                process.SetProcessParameters(V[i], H[i], T[i], Tp[i], Tz[i]);
                waitingList.Add(process);
            }
        }

        public void ExecuteTasks()
        {
            if (!runningList.Any() && !loadingList.Any() && !waitingList.Any()) { allDone = true; }
            else
            {
                ManageWaitingTasks(overallTime);
                ManageLoadingTasks(overallTime);

                remainingH.Add(remainingH[overallTime]);
                remainingV.Add(remainingV[overallTime]);

                ManageRunningTasks(overallTime);

                overallTime++;
            }
        }

        public void FillMultiProcessLists(int i, string state)
        {
            multiTaskDictionary[i].Add(state);
        }

        public void FillMultiTaskDictionary()
        {
            List<string> multiTaskList;
            //fill multi task dictionary of 
            for (int i = 0; i < waitingList.Count; i++) {
                multiTaskList = new List<string>();
                multiTaskDictionary.Add(i, multiTaskList);
            }
        }

        public void ManageRunningTasks(int currentTime)
        {
            multiProcessState.Add(runningList.Count);
            for (int i = 0; i < runningList.Count; i++)
            {
                if (runningList[i].GetT() == 0)
                {
                    remainingV[overallTime + 1] = (remainingV[overallTime] + runningList[i].GetV());
                    remainingH[overallTime + 1] = (remainingH[overallTime] + runningList[i].GetH());

                    multiTaskDictionary[runningList[i].GetID()].Add("Out");
                    runningList[i].SetRealEndT(overallTime);
                    completedList.Add(runningList[i]);
                    runningList.RemoveAt(i);
                }
                else
                {
                    multiTaskDictionary[runningList[i].GetID()].Add("R" + runningList[i].GetT());
                    runningList[i].DecreaseT(1 / Convert.ToDouble(runningList.Count));                    
                }
            }
        }

        public void ManageLoadingTasks(int currentTime)
        {
            for (int i = 0; i < loadingList.Count; i++)
            {
                if (loadingList[i].GetTz() == 0)
                {
                    loadingList[i].SetRealStartT(overallTime);
                    runningList.Add(loadingList[i]);
                    loadingList.RemoveAt(i);
                }
                else
                {
                    multiTaskDictionary[loadingList[i].GetID()].Add("L" + loadingList[i].GetTz().ToString());
                    loadingList[i].DecreaseTz();
                }
            }
        }

        public virtual void ManageWaitingTasks(int currentTime)
        {
            for (int i = 0; i < waitingList.Count; i++)
            { 
                if (currentTime >= waitingList[i].GetTp())
                {
                    //check if enough resourses
                    if (remainingV[overallTime] >= waitingList[i].GetV() && remainingH[overallTime] >= waitingList[i].GetH())
                    {
                        remainingV[overallTime] = (remainingV[overallTime] - waitingList[i].GetV());
                        remainingH[overallTime] = (remainingH[overallTime] - waitingList[i].GetH());
                        waitingList[i].SetRealTp(overallTime);
                        loadingList.Add(waitingList[i]);
                        waitingList.RemoveAt(i);
                        i--;
                    }
                    else
                    {
                        multiTaskDictionary[waitingList[i].GetID()].Add("W");
                    }
                }
                else
                {
                    multiTaskDictionary[waitingList[i].GetID()].Add("");
                }
            }
        }
    

        public void ProcessExecutionState(Process process, int overallTime)
        {
            //bool flag = false;
            //process is NOT running
            if (process.GetState() == 0)
            {
                if (overallTime == process.GetTp())
                {
                    process.ChangeExecuteState();
                }
            }
            //process is waiting
            if (process.GetState() == 1)
            {
                process.ChangeExecuteState();
            }
            //process is loading
            if (process.GetState() == 2)
            {
                if (process.GetTz() == 0)
                {
                    process.ChangeExecuteState();
                }
                if (process.GetInitialTz() == process.GetTz())
                {
                    process.DecreaseTz();
                    remainingV.Add(remainingV[overallTime]);
                    remainingH.Add(remainingH[overallTime]);
                    remainingV[overallTime + 1] = (remainingV[overallTime] - process.GetV());
                    remainingH[overallTime + 1] = (remainingH[overallTime] - process.GetH());
                }
                else
                {
                    process.DecreaseTz();
                    remainingV.Add(remainingV[overallTime]);
                    remainingH.Add(remainingH[overallTime]);
                }
            }
            //process is running
            if (process.GetState() == 3)
            {
                if (process.GetT() == 0)
                {
                    process.ChangeExecuteState();
                    //processList.Remove(process);
                }
                else
                {
                    process.DecreaseT(1);
                    remainingV.Add(remainingV[overallTime]);
                    remainingH.Add(remainingH[overallTime]);
                }
            }
        }

    }
}
