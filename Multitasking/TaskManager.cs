using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    public interface ITaskManager
    {
        int[] GetX();
        int[] GetK();
        int[] GetTp();
        int[] GetV();
        int[] GetH();
        int[] GetT();
        int[] GetTz();
        int[] GetRealTp();
        int[] GetRealStartT();
        int[] GetRealEndT();
        int[] GetActiveT();
        double[] GetWi();
        List<int> GetVHistory();
        List<int> GetHHistory();
        Dictionary<int, List<string>> GetDictionary();
        bool GetAllDone();
        List<int> GetMultiprocState();
        void CalculateX();
        void CalculateK();
        void CalculateTp();
        void CalculateVHTTz();
        void ExecuteTasks();
        void FillMultiProcessLists(int i, string state);
        void FillMultiTaskDictionary();
        void ManageRunningTasks(int currentTime);
        void ManageLoadingTasks(int currentTime);
        void ManageWaitingTasks(int currentTime);
        void ProcessExecutionState(Process process, int overallTime);
        
    }
}
