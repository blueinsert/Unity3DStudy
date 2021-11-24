using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StringCompare
{
    class Program
    {
        public static void StringCompare(string str1,string str2)
        {
            var res = string.Compare(str1, str2);
            Console.WriteLine(string.Format("res:{0}", res));
            var chars1 = str1.ToCharArray();
            var chars2 = str2.ToCharArray();
            if (chars1.Length != chars2.Length)
            {
                Console.WriteLine(string.Format("chars1.Length != chars2.Length"));
                return;
            }
            for(int i = 0; i < chars1.Length; i++)
            {
                if (chars1[i] != chars2[i])
                {
                    Console.WriteLine(string.Format("chars1 {0} != chars2 {1}", chars1[i], chars2[i]));
                }
            }
        }

        static void Main(string[] args)
        {
            var string1 = @"[被动]对战“圣职”部队时，英雄攻击、防御、魔防提升<color=#DC143C>15%</color>，自身英雄兵种为“魔物”时，该效果提升为<color=#DC143C>60%</color>。
[物理伤害]对指定位置<color=#DC143C>3</color>格范围内所有敌军造成<color=#DC143C>0.15</color>倍范围伤害，并召唤阿鲁哈萨托之影。同时使自身部队攻击提升<color=#DC143C>20%</color>，并获得[免疫]效果，持续<color=#DC143C>2</color>回合，使用后可以额外行动<color=#DC143C>3</color>格，可再次攻击。"

;
            var string2 = @"[被动]对战“僧侣”部队时，英雄攻击、防御、魔防提升<color=#DC143C>15%</color>，自身英雄兵种为“魔物”时，该效果提升为<color=#DC143C>60%</color>。
[物理伤害]对指定位置<color=#DC143C>3</color>格范围内所有敌军造成<color=#DC143C>0.15</color>倍范围伤害，并召唤阿鲁哈萨托之影。同时使自身部队攻击提升<color=#DC143C>20%</color>，并获得[免疫]效果，持续<color=#DC143C>2</color>回合，使用后可以额外行动<color=#DC143C>3</color>格，可再次攻击。";
            StringCompare(string1, string2);
        }
    }
}
