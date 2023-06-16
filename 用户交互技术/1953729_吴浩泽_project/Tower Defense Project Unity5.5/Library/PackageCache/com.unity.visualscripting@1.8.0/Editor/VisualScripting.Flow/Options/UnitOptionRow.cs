using System;
using Unity.VisualScripting.Dependencies.Sqlite;

namespace Unity.VisualScripting
{
    public sealed class UnitOptionRow
    {
        [AutoIncrement, PrimaryKey]
        public int id { get; set; }

        public string sourceScriptGuids { get; set; }

        public string optionType { get; set; }
        public string unitType { get; set; }
        public string labelHuman { get; set; }
        public string labelProgrammer { get; set; }
        public string category { get; set; }
        public int order { get; set; }
        public string haystackHuman { get; set; }
        public string haystackProgrammer { get; set; }
        public string favoriteKey { get; set; }
        public string tag1 { get; set; }
        public string tag2 { get; set; }
        public string tag3 { get; set; }
        public string unit { get; set; }

        public int controlInputCount { get; set; }
        public int controlOutputCount { get; set; }
        public string valueInputTypes { get; set; }
        public string valueOutputTypes { get; set; }

        public IUnitOption ToOption()
        {
            using (ProfilingUtility.SampleBlock("Row to option"))
            {
                var optionType = Codebase.DeserializeType(this.optionType);

                IUnitOption option;

                option = (IUnitOption)Activator.CreateInstance(optionType);

                option.Deserialize(this);

                return option;
            }
        }
    }
}
