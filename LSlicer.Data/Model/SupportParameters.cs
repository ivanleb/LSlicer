using LSlicing.Data.Interaction.Contracts;


namespace LSlicer.Data.Model
{
    [System.Serializable]
    public class SupportParameters : ISupportParameters
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public SupportType Type { get; set; }
        public double XGridCellSize { get;set; } //расстояния между центрами поддержек
        public double YGridCellSize { get; set; }
        public double XElementWidth { get; set; }//толщина поддержек
        public double YElementWidth { get; set; }
        public bool UseZoneofInterests { get; set; }
        public bool EndToEndSupport { get; set; }
        public double StartZonePointX { get; set; }//начальная точка
        public double StartZonePointY { get; set; }
        public double StartZonePointZ { get; set; }
        public double LenghtZoneX { get; set; }
        public double LenghtZoneY { get; set; }
        public double LenghtZoneZ { get; set; }
        public double PartIntersectionDeep { get; set; }//углубление в деталь 
        public double ZGridCellSize { get; set; }//высота ячейки сеточной поддержки
        public double HeadLength { get; set; }//длинна головки поддержек
        public int Angle { get; set; }//угол отвертикали, если нормаль треугольника лежит под углом больше Angle, то треугольник дальше не рассматривается
        public bool IsSupportsForPartContour { get; set; }//построение поддержек по контуру
        public bool IsSupportsForPartBody { get; set; }//построение основныз поддержек
        public double WeightFunction { get; set; }//использование весовой функции
        public double MinLength { get; set; }//минимальная длинна поддержки

        public object Clone()
        {
            return (SupportParameters)this.MemberwiseClone();
        }
    }
}
