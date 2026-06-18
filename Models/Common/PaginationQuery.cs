namespace FirinApi.Models.Common;

public class PaginationQuery
{
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 10;

    public int Skip => (Page - 1) * Size;

    public void Normalize(int maxSize = 100)
    {
        if (Page < 1) Page = 1;
        if (Size < 1) Size = 10;
        if (Size > maxSize) Size = maxSize;
    }
}
