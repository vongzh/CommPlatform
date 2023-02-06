using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Zk.HotelPlatform.Utils
{
    public class GenerateUtil
    {
        public static string getEnName()
        {
            string name = string.Empty;
            string[] currentConsonant;
            string[] vowels = "a,a,a,a,a,e,e,e,e,e,e,e,e,e,e,e,i,i,i,o,o,o,u,y,ee,ee,ea,ea,ey,eau,eigh,oa,oo,ou,ough,ay".Split(',');
            string[] commonConsonants = "s,s,s,s,t,t,t,t,t,n,n,r,l,d,sm,sl,sh,sh,th,th,th".Split(',');
            string[] averageConsonants = "sh,sh,st,st,b,c,f,g,h,k,l,m,p,p,ph,wh".Split(',');
            string[] middleConsonants = "x,ss,ss,ch,ch,ck,ck,dd,kn,rt,gh,mm,nd,nd,nn,pp,ps,tt,ff,rr,rk,mp,ll".Split(','); //Can't start
            string[] rareConsonants = "j,j,j,v,v,w,w,w,z,qu,qu".Split(',');
            Random rng = new Random(Guid.NewGuid().GetHashCode()); //http://codebetter.com/blogs/59496.aspx
            int[] lengthArray = new int[] { 2, 2, 2, 2, 2, 2, 3, 3, 3, 4 }; //Favor shorter names but allow longer ones
            int length = lengthArray[rng.Next(lengthArray.Length)];
            for (int i = 0; i < length; i++)
            {
                int letterType = rng.Next(1000);
                if (letterType < 775) currentConsonant = commonConsonants;
                else if (letterType < 875 && i > 0) currentConsonant = middleConsonants;
                else if (letterType < 985) currentConsonant = averageConsonants;
                else currentConsonant = rareConsonants;
                name += currentConsonant[rng.Next(currentConsonant.Length)];
                name += vowels[rng.Next(vowels.Length)];
                if (name.Length > 4 && rng.Next(1000) < 800) break; //Getting long, must roll to save
                if (name.Length > 6 && rng.Next(1000) < 950) break; //Really long, roll again to save
                if (name.Length > 7) break; //Probably ridiculous, stop building and add ending
            }
            int endingType = rng.Next(1000);
            if (name.Length > 6)
                endingType -= (name.Length * 25); //Don't add long endings if already long
            else
                endingType += (name.Length * 10); //Favor long endings if short
            if (endingType < 400) { } // Ends with vowel
            else if (endingType < 775) name += commonConsonants[rng.Next(commonConsonants.Length)];
            else if (endingType < 825) name += averageConsonants[rng.Next(averageConsonants.Length)];
            else if (endingType < 840) name += "ski";
            else if (endingType < 860) name += "son";
            else if (Regex.IsMatch(name, "(.+)(ay|e|ee|ea|oo)$") || name.Length < 5)
            {
                name = "Mc" + name.Substring(0, 1).ToUpper() + name.Substring(1);
                return name;
            }
            else name += "ez";
            name = name.Substring(0, 1).ToUpper() + name.Substring(1); //Capitalize first letter
            return name;
        }


        /**
       * 获取中国人姓名
       *
       * @return
       */
        public static string getName(int seed)
        {
            string[] surname = {"赵", "钱", "孙", "李", "周", "吴", "郑", "王", "冯", "陈", "褚", "卫", "蒋", "沈", "韩", "杨", "朱", "秦", "尤", "许",
                "何", "吕", "施", "张", "孔", "曹", "严", "华", "金", "魏", "陶", "姜", "戚", "谢", "邹", "喻", "柏", "水", "窦", "章", "云", "苏", "潘", "葛", "奚", "范", "彭", "郎",
                "鲁", "韦", "昌", "马", "苗", "凤", "花", "方", "俞", "任", "袁", "柳", "酆", "鲍", "史", "唐", "费", "廉", "岑", "薛", "雷", "贺", "倪", "汤", "滕", "殷",
                "罗", "毕", "郝", "邬", "安", "常", "乐", "于", "时", "傅", "皮", "卞", "齐", "康", "伍", "余", "元", "卜", "顾", "孟", "平", "黄", "和",
                "穆", "萧", "尹", "姚", "邵", "湛", "汪", "祁", "毛", "禹", "狄", "米", "贝", "明", "臧", "计", "伏", "成", "戴", "谈", "宋", "茅", "庞", "熊", "纪", "舒",
                "屈", "项", "祝", "董", "梁", "杜", "阮", "蓝", "闵", "席", "季"};
            string girl = "秀娟英华慧巧美娜静淑惠珠翠雅芝玉萍红娥玲芬芳燕彩春菊兰凤洁梅琳素云莲真环雪荣爱妹霞香月莺媛艳瑞凡佳嘉琼勤珍贞莉桂娣叶璧璐娅琦晶妍茜秋珊莎锦黛青倩婷姣婉娴瑾颖露瑶怡婵雁蓓纨仪荷丹蓉眉君琴蕊薇菁梦岚苑婕馨瑗琰韵融园艺咏卿聪澜纯毓悦昭冰爽琬茗羽希宁欣飘育滢馥筠柔竹霭凝晓欢霄枫芸菲寒伊亚宜可姬舒影荔枝思丽 ";
            string boy = "伟刚勇毅俊峰强军平保东文辉力明永健世广志义兴良海山仁波宁贵福生龙元全国胜学祥才发武新利清飞彬富顺信子杰涛昌成康星光天达安岩中茂进林有坚和彪博诚先敬震振壮会思群豪心邦承乐绍功松善厚庆磊民友裕河哲江超浩亮政谦亨奇固之轮翰朗伯宏言若鸣朋斌梁栋维启克伦翔旭鹏泽晨辰士以建家致树炎德行时泰盛雄琛钧冠策腾楠榕风航弘";

            var random = new Random(seed);
            //获得一个随机的姓氏
            string name = surname[random.Next(surname.Length - 1)];
            //可以根据这个数设置产生的男女比例
            int i = random.Next(10);
            if (i == 2 || i == 4 || i == 6)
            {
                int j = random.Next(girl.Length - 2);
                if (j % 2 == 0)
                {
                    name = name + girl.Substring(j, 2);
                }
                else
                {
                    name = name + girl.Substring(j, 1);
                }

            }
            else
            {
                int j = random.Next(boy.Length - 2);
                if (j % 2 == 0)
                {
                    name = name + boy.Substring(j, 2);
                }
                else
                {
                    name = name + boy.Substring(j, 1);
                }
            }

            return name;
        }


        /**
     * 获取身份证号
     * @return
     */
        public static string getRandomID(int seed)
        {
            // 随机生成省、自治区、直辖市代码 1-2
            string[] provinces = { "11", "12", "13", "14", "15", "21", "22", "23",
                "31", "32", "33", "34", "35", "36", "37", "41", "42", "43",
                "44", "45", "46", "50", "51", "52", "53", "54", "61", "62",
                "63", "64", "65", "71", "81", "82" };
            string province = randomOne(provinces, seed);
            // 随机生成地级市、盟、自治州代码 3-4
            string city = randomCityCode(18, seed);
            // 随机生成县、县级市、区代码 5-6
            string county = randomCityCode(28, seed);
            // 随机生成出生年月 7-14
            string birth = randomBirth(seed);
            // 随机生成顺序号 15-17(随机性别)
            string no = new Random(seed).Next(899) + 100 + "";

            // 随机生成校验码 18
            //string[] checks = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9",
            //    "X" };
            //string check = randomOne(checks,seed);

            string check = GenParityBit(province + city + county + birth + no);
            // 拼接身份证号码
            return province + city + county + birth + no + check;
        }

        private static string GenParityBit(string s17)
        {
            int[] Weight = new int[] { 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2 };

            string Parity = "10X98765432";

            int s = 0;

            for (int i = 0; i < s17.Length; i++)
            {
                s += Int32.Parse(s17[i].ToString()) * Weight[i];
            }

            return Parity[s % 11].ToString();//返回校验码

        }

        /**
         * 随机生成minAge到maxAge年龄段的人的生日日期
         *
         * @author mingzijian
         * @param minAge
         * @param maxAge
         * @return
         */
        private static string randomBirth(int seed)
        {
            var random = new Random(seed);

            var year = random.Next(1970, 2000);
            var month = random.Next(1, 12);
            var day = random.Next(1, 28);

            string m = month.ToString().PadLeft(2, '0');
            string d = day.ToString().PadLeft(2, '0');

            return year + m + d;
        }

        /**
         * 随机生成两位数的字符串（01-max）,不足两位的前面补0
         *
         * @author mingzijian
         * @param max
         * @return
         */
        private static string randomCityCode(int max, int seed)
        {
            int i = new Random(seed).Next(max) + 1;
            return i.ToString().PadLeft(2, '0');
        }


        /**
         * 从String[] 数组中随机取出其中一个String字符串
         *
         * @author mingzijian
         * @param s
         * @return
         */
        private static string randomOne(string[] s, int seed)
        {
            return s[new Random(seed).Next(s.Length - 1)];
        }
    }
}
