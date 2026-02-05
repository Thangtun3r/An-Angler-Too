namespace Yarn.Unity.ExposedVariables {

    using Yarn.Unity;

    [System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.1.3.0")]
    public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
        // Accessor for Bool $failedFishing
        public bool FailedFishing {
            get => this.GetValueOrDefault<bool>("$failedFishing");
            set => this.SetValue<bool>("$failedFishing", value);
        }

        // Accessor for Bool $gotSteeringWheel
        public bool GotSteeringWheel {
            get => this.GetValueOrDefault<bool>("$gotSteeringWheel");
            set => this.SetValue<bool>("$gotSteeringWheel", value);
        }

    }
}
