using System;

namespace IntBoolConverter {
    public static class IntBoolConvert{
        public static bool ToBool (int i) {
            return Convert.ToBoolean (i);
        }

        public static int ToInt(bool b){
            return Convert.ToInt32(b);
        }
    }
}