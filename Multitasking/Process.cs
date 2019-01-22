using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FIFO
{
    public class Process : IComparable<Process>
    {
        int ID;

        int V;
        int H;

        double T;
        double initialT;
        int Tp;
        int Tz;
        int initialTz;

        int realTp;
        int realStartT;
        int realEndT;

        int endTz;
        int endT;

        bool endFlag;
        byte executeFlag = 0;

        public int CompareTo(Process other)
        {
            return this.GetID().CompareTo(other.GetID());
            throw new NotImplementedException();
        }

        public Process(int ID)
        {
            this.V = 0;
            this.H = 0;
            this.T = 0;
            this.Tp = 0;
            this.Tz = 0;
            this.ID = ID;
        }

        public void SetEndTz(int endTz)
        {
            this.endTz = endTz;
        }

        public void SetEndT(int endT)
        {
            this.endT = endT;
        }

        public void SetRealTp(int realTp)
        {
            this.realTp = realTp;
        }

        public void SetRealStartT(int realStartT)
        {
            this.realStartT = realStartT;
        }

        public void SetRealEndT(int realEndT)
        {
            this.realEndT = realEndT;
        }

        public void SetProcessParameters(int V, int H, int T, int Tp, int Tz)
        {
            this.V = V;
            this.H = H;
            this.T = T;
            this.Tp = Tp;
            this.Tz = Tz;
            this.initialTz = Tz;
            this.initialT = T;
        }


        public void ChangeExecuteState()
        {
            if (executeFlag <= 3)
            {
                executeFlag++;
            }
            else
            {
                endFlag = true;
            }
        }

        public void DecreaseTp() { Tp--; }

        public void DecreaseT(double t)
        {
            T -= Math.Round(t, 2);
            T = Math.Round(T, 2);

            if (T < 0.2)
            {
                T = 0;
            }
        }

        public void DecreaseTz() { Tz--; }

        public int GetEndTz() { return endTz; }

        public int GetEndT() { return endT; }

        public int GetState() { return executeFlag; }

        public bool GetEndFlag() { return endFlag; }

        public int GetID() { return ID; }

        public int GetV() { return V; }

        public int GetH() { return H; }

        public double GetT() { return T; }

        public int GetTp() { return Tp; }

        public int GetTz() { return Tz; }

        public int GetInitialTz() { return initialTz; }

        public int GetRealTp() { return realTp; }

        public int GetRealStartT() { return realStartT; }

        public int GetRealEndT() { return realEndT; }

        public int GetActiveTime() { return realEndT - realStartT; }

        public double GetWi()
        {
            double wi = (realEndT - realTp) / initialT;
            wi = Math.Round(wi, 2);
            return wi;
        }
    }
}
