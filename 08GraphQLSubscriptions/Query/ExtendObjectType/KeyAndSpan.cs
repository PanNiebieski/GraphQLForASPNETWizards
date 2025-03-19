namespace _08GraphQLSubscriptions.Query.ExtendObjectType;

public class KeyAndSpan : IEquatable<KeyAndSpan>
{
    public KeyAndSpan(string key, ChangeSpan span)
    {
        Key = int.Parse(key);
        Span = span;
    }

    public KeyAndSpan(int key, ChangeSpan span)
    {
        Key = key;
        Span = span;
    }

    public int Key { get; set; }
    public ChangeSpan Span { get; set; }

    // It's important to override Equals and GetHashCode for key equality in dictionary lookups.
    public override bool Equals(object? obj)
    {
        if (obj is not KeyAndSpan other)
            return false;

        return Key == other.Key && Span.Equals(other.Span);
    }

    public bool Equals(KeyAndSpan? other)
    {
        if (other == null)
            return false;

        return Key == other.Key && Span.Equals(other.Span);
    }

    public override int GetHashCode() => HashCode.Combine(Key, Span);
}
