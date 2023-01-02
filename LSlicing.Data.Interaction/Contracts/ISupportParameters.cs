using System;

namespace LSlicer.Data.Interaction.Contracts
{

    public interface ISupportParameters : ICloneable, IIdentifier, IHasName
    {
        string Name { get; set; }
        SupportType Type { get; set; }
        double XGridCellSize { get; set; } //расст
        double YGridCellSize { get; set; }
        double XElementWidth { get; set; }
        double YElementWidth { get; set; }
        bool EndToEndSupport { get; set; }
        bool UseZoneofInterests { get; set; }
        double StartZonePointX { get; set; }//начальная точка
        double StartZonePointY { get; set; }
        double StartZonePointZ { get; set; }
        double LenghtZoneX { get; set; }
        double LenghtZoneY { get; set; }
        double LenghtZoneZ { get; set; }
        double PartIntersectionDeep { get; set; }
        double ZGridCellSize { get; set; }//в
        double HeadLength { get; set; }//длин
        int Angle { get; set; }//угол отверти
        bool IsSupportsForPartContour { get; set; }
        bool IsSupportsForPartBody { get; set; }
        double WeightFunction { get; set; }
        double MinLength { get; set; }
    }

    public static class SupportParametersExtentions
    {
        public static string GetIdentifier(this ISupportParameters parameters)
        {
            return parameters.Name;
        }
    }
}