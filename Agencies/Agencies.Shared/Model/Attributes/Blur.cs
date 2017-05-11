﻿namespace Agencies.Shared
{
    public class Blur : Attribute
    {
        public string BlurLevel { get; set; }

        public float Value { get; set; }

        public override string ToString ()
        {
            return $"Blur Level: {BlurLevel} ({Value})";
        }
    }
}