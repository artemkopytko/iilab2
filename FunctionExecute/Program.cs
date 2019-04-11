using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace FunctionExecute
{
    public class Params
    {
        public class Param
        {
            double value;
            string name;
            string id;
            bool isCalculated;

            public Param(double v, bool isCalcs)
            {
                Value = v;
                IsCalculated = isCalcs;
            }

            public Param()
            {
                Value = 0;
                IsCalculated = false;
            }

            public static Param parseFromString(string str)
            {
                Param prm = new Param(0, false);

                string[] strs = str.Split(new char[] { ':' });
                prm.value = double.Parse(strs[0]);
                prm.Id = strs[1].Trim();
                prm.Name = strs[2].Trim();

                if (prm.value != 0)
                {
                    prm.isCalculated = true;
                }

                return prm;
            }

            public double Value { get => value; set => this.value = value; }
            public bool IsCalculated { get => isCalculated; set => isCalculated = value; }
            public string Name { get => name; set => name = value; }
            public string Id { get => id; set => id = value; }

            public override string ToString()
            {
                return Value + " " + Id + " " + Name;
            }
        }

        List<Param> _params = new List<Param>();

        public List<Param> AllParams { get => _params; set => _params = value; }

        public void readFromFile(string path)
        {
            try
            {
                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    bool wasD1 = false;
                    while ((line = sr.ReadLine()) != null && line != "#2")
                    {
                        if (line == "#1")
                        {
                            wasD1 = true;
                            continue;
                        }
                        if (wasD1)
                        {
                            _params.Add(Param.parseFromString(line));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public void printCalculatedParams()
        {
            Console.WriteLine("Calculated params");
            foreach (Param pr in AllParams)
            {
                if (pr.IsCalculated)
                {
                    Console.WriteLine(pr);
                }
            }
        }

        public Param getById(string id)
        {
            return _params.Find(x => x.Id == id);
        }
    }

    public class Functions
    {
        public class Function
        {
            Dictionary<string, Params.Param> my_param;
            Params.Param result;
            string name;
            string description;

            public Function()
            {
                My_param = new Dictionary<string, Params.Param>();
            }

            public Params.Param Result { get => result; set => result = value; }
            public string Name { get => name; set => name = value; }
            public Dictionary<string, Params.Param> My_param { get => my_param; set => my_param = value; }
            public string Description { get => description; set => description = value; }
            public bool isContainParam(List<Params.Param> un_params)
            {
                foreach (Params.Param pr in un_params)
                {
                    if (my_param.ContainsKey(pr.Name))
                    {
                        return true;
                    }
                }
                return false;
            }
            public static Function parseFromString(string str, Params prms)
            {
                Function prm = new Function();

                string[] idAndFuncName = str.Split(new char[] { '=' });
                prm.Result = prms.getById(idAndFuncName[0]);

                string[] NameAndParams = idAndFuncName[1].Split(new char[] { '(', ')' });
                prm.Name = NameAndParams[0];

                string[] func_params = NameAndParams[1].Split(new char[] { ',' });

                foreach (string s in func_params)
                {
                    prm.My_param[s.Trim()] = prms.getById(s.Trim());
                }

                prm.Description = NameAndParams[2];

                return prm;
            }

        }

        List<Function> _params = new List<Function>();
        Params param = new Params();

        public List<Function> AllParams { get => _params; set => _params = value; }
        public Params Param { get => param; set => param = value; }

        public void readFromFile(string path)
        {
            try
            {
                Param.readFromFile(path);
                Param.printCalculatedParams();

                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    bool wasD2 = false;
                    while ((line = sr.ReadLine()) != null && line != "#3")
                    {
                        if (line == "#2")
                        {
                            wasD2 = true;
                            continue;
                        }
                        if (wasD2)
                        {
                            _params.Add(Function.parseFromString(line, Param));
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        public List<Function> findFuncByResult(Params.Param prm)
        {
            return _params.FindAll(x => x.Result.Id == prm.Id);
        }

    }

    public class Calculator
    {

        Functions funcs_info;
        List<Params.Param> unknowsParams = new List<Params.Param>();

        public Calculator(Functions funcs)
        {
            funcs_info = funcs;
        }

        public double ConvertToRadians(double angle)
        {
            return (Math.PI / 180) * angle;
        }

        double F11(Params.Param h)
        {
            if (!h.IsCalculated)
            {
                h.Value = calc(h.Id);
            }

            if (!h.IsCalculated)
            {
                return 0;
            }

            return h.Value / 2;
        }
        double F12(Params.Param bd, Params.Param ac, Params.Param a)
        {
            if (!bd.IsCalculated)
            {
                bd.Value = calc(bd.Id);
            }

            if (!ac.IsCalculated)
            {
                ac.Value = calc(ac.Id);
            }
            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!bd.IsCalculated || !ac.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return bd.Value * ac.Value / 4 * a.Value;
        }

        double F13(Params.Param a, Params.Param beta)
        {
            if (!beta.IsCalculated)
            {
                beta.Value = calc(beta.Id);
            }


            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!beta.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return a.Value * Math.Sqrt(2 - (2 * Math.Cos(ConvertToRadians(beta.Value))));
        }

        double F14(Params.Param a, Params.Param alpha)
        {
            if (!alpha.IsCalculated)
            {
                alpha.Value = calc(alpha.Id);
            }


            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!alpha.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return a.Value * Math.Sqrt(2 + (2 * Math.Cos(ConvertToRadians(alpha.Value))));
        }

        double F15(Params.Param a, Params.Param alpha)
        {
            if (!alpha.IsCalculated)
            {
                alpha.Value = calc(alpha.Id);
            }


            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!alpha.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return a.Value * Math.Sqrt(2 - (2 * Math.Cos(ConvertToRadians(alpha.Value))));
        }

        double F16(Params.Param a, Params.Param beta)
        {
            if (!beta.IsCalculated)
            {
                beta.Value = calc(beta.Id);
            }


            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!beta.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return a.Value * Math.Sqrt(2 + (2 * Math.Cos(ConvertToRadians(beta.Value))));
        }

        double F17(Params.Param r)
        {
            if (!r.IsCalculated)
            {
                r.Value = calc(r.Id);
            }

            return Math.PI * Math.Pow(r.Value, 2);
        }

        double F18(Params.Param r)
        {
            if (!r.IsCalculated)
            {
                r.Value = calc(r.Id);
            }

            return 2 * Math.PI * r.Value;
        }

        double F19(Params.Param a, Params.Param h)
        {
            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }


            if (!h.IsCalculated)
            {
                h.Value = calc(h.Id);
            }

            if (!h.IsCalculated || !a.IsCalculated)
            {
                return 0;
            }

            return a.Value * h.Value;
        }

        double F20(Params.Param bd, Params.Param ac)
        {
            if (!bd.IsCalculated)
            {
                bd.Value = calc(bd.Id);
            }


            if (!ac.IsCalculated)
            {
                ac.Value = calc(ac.Id);
            }

            if (!ac.IsCalculated || !bd.IsCalculated)
            {
                return 0;
            }

            return ac.Value * bd.Value / 2;
        }

        double F21(Params.Param a)
        {
            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            return 4 * a.Value;
        }
        double F22(Params.Param Sr, Params.Param h)
        {
            if (!Sr.IsCalculated)
            {
                Sr.Value = calc(Sr.Id);
            }


            if (!h.IsCalculated)
            {
                h.Value = calc(h.Id);
            }

            if (!Sr.IsCalculated || !h.IsCalculated)
            {
                return 0;
            }

            return Sr.Value / h.Value;
        }

        double F23(Params.Param bd, Params.Param ac)
        {
            if (!bd.IsCalculated)
            {
                bd.Value = calc(bd.Id);
            }


            if (!ac.IsCalculated)
            {
                ac.Value = calc(ac.Id);
            }

            if (!bd.IsCalculated || !ac.IsCalculated)
            {
                return 0;
            }

            return 0.5 * Math.Sqrt(Math.Pow(bd.Value, 2) + Math.Pow(ac.Value, 2));
        }

        double F24(Params.Param Sr, Params.Param a)
        {
            if (!Sr.IsCalculated)
            {
                Sr.Value = calc(Sr.Id);
            }


            if (!a.IsCalculated)
            {
                a.Value = calc(a.Id);
            }

            if (!a.IsCalculated || !Sr.IsCalculated)
            {
                return 0;
            }

            return Sr.Value / a.Value;
        }

        double F25(Params.Param Pr)
        {
            if (!Pr.IsCalculated)
            {
                Pr.Value = calc(Pr.Id);
            }


            if (!Pr.IsCalculated)
            {
                return 0;
            }

            return Pr.Value / 4;
        }
        double F26(Params.Param r)
        {
            if (!r.IsCalculated)
            {
                r.Value = calc(r.Id);
            }


            if (!r.IsCalculated)
            {
                return 0;
            }

            return 2 * r.Value;
        }

        public double calc(string id)
        {
            double result = 0;

            Params.Param p = funcs_info.Param.getById(id);

            if (p.IsCalculated)
            {
                return p.Value;
            }

            unknowsParams.Add(p);
            List<Functions.Function> funcs = funcs_info.findFuncByResult(p);
            foreach (var func in funcs)
            {
                if (func.isContainParam(unknowsParams))
                {
                    continue;
                }

                Console.WriteLine("Calculating {0}", func.Description);
                switch (func.Name)
                {
                    case "F11":
                        {
                            result = this.F11(func.My_param["h"]);
                            break;
                        }
                    case "F12":
                        {
                            result = this.F12(func.My_param["bd"], func.My_param["ac"], func.My_param["a"]);
                            break;
                        }
                    case "F13":
                        {
                            result = this.F13(func.My_param["a"], func.My_param["beta"]);
                            break;
                        }
                    case "F14":
                        {
                            result = this.F14(func.My_param["a"], func.My_param["alpha"]);
                            break;
                        }
                    case "F15":
                        {
                            result = this.F15(func.My_param["a"], func.My_param["alpha"]);
                            break;
                        }
                    case "F16":
                        {
                            result = this.F16(func.My_param["a"], func.My_param["beta"]);
                            break;
                        }
                    case "F17":
                        {
                            result = this.F17(func.My_param["r"]);
                            break;
                        }
                    case "F18":
                        {
                            result = this.F18(func.My_param["r"]);
                            break;
                        }
                    case "F19":
                        {
                            result = this.F19(func.My_param["a"], func.My_param["h"]);
                            break;
                        }
                    case "F20":
                        {
                            result = this.F20(func.My_param["bd"], func.My_param["ac"]);
                            break;
                        }
                    case "F21":
                        {
                            result = this.F21(func.My_param["a"]);
                            break;
                        }
                    case "F22":
                        {
                            result = this.F22(func.My_param["Sr"], func.My_param["h"]);
                            break;
                        }
                    case "F23":
                        {
                            result = this.F23(func.My_param["bd"], func.My_param["ac"]);
                            break;
                        }
                    case "F24":
                        {
                            result = this.F24(func.My_param["Sr"], func.My_param["a"]);
                            break;
                        }
                    case "F25":
                        {
                            result = this.F25(func.My_param["Pr"]);
                            break;
                        }
                    case "F26":
                        {
                            result = this.F26(func.My_param["r"]);
                            break;
                        }
                }
                Console.WriteLine(result);
                if (result == 0)
                {
                    continue;
                }
                Console.WriteLine("Result {0}: {1}", func.Description, result);
                break;
            }
            if (result != 0)
            {
                p.Value = result;
                p.IsCalculated = true;
                unknowsParams.Clear();
            }
            return result;

        }
    }

    class Program
    {
        static List<string> loadCalcQueue(string path)
        {
            List<string> queue = new List<string>();

            try
            {

                using (StreamReader sr = new StreamReader(path, System.Text.Encoding.Default))
                {
                    string line;
                    bool wasD3 = false;
                    while ((line = sr.ReadLine()) != null)
                    {
                        if (line == "#3")
                        {
                            wasD3 = true;
                            continue;
                        }
                        if (wasD3)
                        {
                            queue.Add(line.Trim());
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return queue;
        }

        static void Main(string[] args)
        {
            try
            {
                Functions func = new Functions();
                func.readFromFile("Data.txt");

                Calculator calculator = new Calculator(func);
                var queue = loadCalcQueue("Data.txt");

                foreach (var id in queue)
                {
                    calculator.calc(id);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("ERROR: " + e.Message);
            }
        }
    }
}
