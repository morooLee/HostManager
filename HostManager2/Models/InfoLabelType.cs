using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    // InfoLabel 상태값
    public enum InfoLabelType
    {
        Warning = 0,    // 경고 (주황색)
        Success = 1,    // 완료 (녹색)
        Info = 2,       // 정보 (하늘색)
        None = 3        // None (흰색)
    }
}
