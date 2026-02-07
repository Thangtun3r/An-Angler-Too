namespace Yarn.Unity.ExposedVariables {

    using Yarn.Unity;

    [System.CodeDom.Compiler.GeneratedCode("YarnSpinner", "3.1.3.0")]
    public partial class YarnVariables : Yarn.Unity.InMemoryVariableStorage, Yarn.Unity.IGeneratedVariableStorage {
        // Accessor for Bool $StarringWheel
        public bool StarringWheel {
            get => this.GetValueOrDefault<bool>("$StarringWheel");
            set => this.SetValue<bool>("$StarringWheel", value);
        }

        // Accessor for Bool $VinEel
        public bool VinEel {
            get => this.GetValueOrDefault<bool>("$VinEel");
            set => this.SetValue<bool>("$VinEel", value);
        }

        // Accessor for Bool $TurnedBlue
        public bool TurnedBlue {
            get => this.GetValueOrDefault<bool>("$TurnedBlue");
            set => this.SetValue<bool>("$TurnedBlue", value);
        }

        // Accessor for Bool $OnRug
        public bool OnRug {
            get => this.GetValueOrDefault<bool>("$OnRug");
            set => this.SetValue<bool>("$OnRug", value);
        }

        // Accessor for Bool $JellyGlitch
        public bool JellyGlitch {
            get => this.GetValueOrDefault<bool>("$JellyGlitch");
            set => this.SetValue<bool>("$JellyGlitch", value);
        }

    }
}
