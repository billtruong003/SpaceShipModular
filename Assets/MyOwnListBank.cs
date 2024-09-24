using System.Collections;
using System.Collections.Generic;
using AirFishLab.ScrollingList;
using AirFishLab.ScrollingList.ContentManagement;
using UnityEngine;
using NaughtyAttributes; // Thêm thư viện NaughtyAttributes

public class ListBank : BaseListBank
{
    // Sử dụng ReorderableList để có thể kéo thả, sắp xếp và chỉnh sửa trực tiếp trên Inspector
    [ReorderableList]
    [SerializeField]
    private int[] contents = {
            1, 2, 3, 4, 5, 6, 7, 8, 9, 10
        };

    private readonly Content _contentWrapper = new Content();

    public override IListContent GetListContent(int index)
    {
        _contentWrapper.Value = contents[index];
        return _contentWrapper;
    }

    public override int GetContentCount()
    {
        return contents.Length;
    }

    // Lớp con Content có thể đánh dấu bằng Expandable để hiển thị nội dung dễ dàng hơn trong Inspector
    [System.Serializable]
    public class Content : IListContent
    {
        [ShowNonSerializedField, ReadOnly]
        public int Value;
    }
}
