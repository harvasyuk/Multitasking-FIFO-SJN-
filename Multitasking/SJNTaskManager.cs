using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    class SJNTaskManager : FIFOTaskManager
    {
        public SJNTaskManager(int zeroX) : base(zeroX) { }

        bool flag = false;

        public override void ManageWaitingTasks(int currentTime)
        {
            if (!flag)
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
                        else if (overallTime > waitingList[waitingList.Count - 1].GetTp()) {
                            flag = true;
                            multiTaskDictionary[waitingList[i].GetID()].Add("W");
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
            else
            {
                for (int i = 0; i < waitingList.Count; i++)
                {
                    int id = FindIDWithLowestT(waitingList);

                    if (currentTime >= waitingList[i].GetTp() && waitingList[i].GetID() == id)
                    {
                        if (remainingV[overallTime] >= waitingList[i].GetV() && remainingH[overallTime] >= waitingList[i].GetH())
                        {
                            remainingV[overallTime] = (remainingV[overallTime] - waitingList[i].GetV());
                            remainingH[overallTime] = (remainingH[overallTime] - waitingList[i].GetH());
                            waitingList[i].SetRealTp(overallTime);
                            loadingList.Add(waitingList[i]);
                            waitingList.RemoveAt(i);
                            flag = true;
                            i--;
                        }
                        else 
                        {
                            multiTaskDictionary[waitingList[i].GetID()].Add("W");
                        }
                    }
                    else if (currentTime > waitingList[i].GetTp())
                    {
                        multiTaskDictionary[waitingList[i].GetID()].Add("W");
                    }
                    else
                    {
                        multiTaskDictionary[waitingList[i].GetID()].Add("");
                    }
                }
            }
            
        }

        private int FindIDWithLowestT(List<Process> processList)
        {
            double lowestT = processList[0].GetT();
            int id = processList[0].GetID();
            foreach (Process process in waitingList)
            {
                if (process.GetT() < lowestT)
                {
                    lowestT = process.GetT();
                    id = process.GetID();
                }
            }
            return id;
        }
    }
}
