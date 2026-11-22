namespace Hospital_VerticalSlice.Shared
{
    public static class UpdateExtentions
    {
        public static bool UpdateIfChanged<T>(this ref T? OldValue, T? NewValue) where T : struct
        {
            if (NewValue != null && !NewValue.Equals(OldValue))
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        public static bool UpdateIfChanged(this int OldValue, int? NewValue)
        {
            if (NewValue != null && NewValue != OldValue)
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        public static bool UpdateIfChanged(this int? OldValue, int? NewValue)
        {
            if (NewValue != null && NewValue != OldValue)
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        public static bool UpdateIfChanged(this float? OldValue, float? NewValue)
        {
            if (NewValue != null && NewValue != OldValue)
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        // fix later (maybe by ref)
        public static bool UpdateIfChanged(this bool? OldValue, bool? NewValue)
        {
            if (NewValue != null && NewValue != OldValue)
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        public static bool UpdateIfChanged(this ref bool OldValue, bool? NewValue)
        {
            if (NewValue != null && NewValue != OldValue)
            {
                OldValue = NewValue.Value;
                return true;
            }
            return false;
        }
        public static bool UpdateIfChanged(this string? OldValue, string? NewValue)
        {
            if (!string.IsNullOrWhiteSpace(NewValue) && NewValue != OldValue)
            {
                OldValue = NewValue;
                return true;
            }
            return false;
        }

    }
}
