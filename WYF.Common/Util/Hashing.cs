﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WYF
{
    public static class Hashing
    {
        public static int Hash(params object[] args)
        {
            unchecked // Overflow is fine, just wrap
            {
                int Hash = 17;

                foreach (object Item in args)
                {
                    if (Item != null)
                    {
                        Hash = (Hash * 23) + Item.GetHashCode();
                    }
                }

                return Hash;
            }
        }
    }
}
