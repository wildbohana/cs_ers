using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Common.Klase
{
    public enum VrstaMerenja
    {
        [EnumMember] ANALOGNO_MERENJE,
        [EnumMember] DIGITALNO_MERENJE
    }
}
