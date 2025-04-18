﻿using _06GraphQLDocumentDataLoader.ExtendObjectType;

namespace _06GraphQLDocumentDataLoader;

public static class Extensions
{
    public static DateTime GetDateFirst(this List<DocumentViewRecord> list)
    {
        if (list.Count == 0)
        {
            return DateTime.MinValue;
        }
        return list[0].Date;
    }

    public static DateTime GetDateLast(this List<DocumentViewRecord> list)
    {
        if (list.Count == 0)
        {
            return DateTime.MinValue;
        }
        return list[list.Count - 1].Date;
    }
}