using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HostManager.Models
{
    // 노드 변경 상태
    public enum NodeEditPosition
    {
        ChildNodeAdd = -1,  // 하위 항목에 추가하기
        ThisNodeEdit = 0,   // 현재 항목에 추가하기 
        SameNodeAdd = 1,    // 선택한 항목 수정하기
        RootNodeAdd = 2,    // Root 항목에 추가하기
        ThisNodeDel = 10    // 선택한 항목 삭제하기
    }
}
