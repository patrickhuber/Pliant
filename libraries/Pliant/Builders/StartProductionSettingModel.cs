using Pliant.Grammars;

namespace Pliant.Builders
{
    public class StartProductionSettingModel
        : SettingModel
    {
        public const string SettingKey = "start";

        public StartProductionSettingModel(ProductionModel productionModel)
            : base(SettingKey, productionModel.LeftHandSide.NonTerminal.Value)
        { }

        public StartProductionSettingModel(FullyQualifiedName fullyQualifiedName)
            : base(SettingKey, fullyQualifiedName.FullName)
        { }
    }
}
