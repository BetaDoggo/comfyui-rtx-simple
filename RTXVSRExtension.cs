using SwarmUI.Core;
using SwarmUI.Utils;
using SwarmUI.Text2Image;
using SwarmUI.Builtin_ComfyUIBackend;
using Newtonsoft.Json.Linq;

namespace RTXVSRExtension;

public class RTXVSRExtension : Extension
{
    public static T2IRegisteredParam<float> ScaleParam;
    public static T2IRegisteredParam<string> QualityParam;
    public static T2IParamGroup RTXVSRGroup;

    public override void OnInit()
    {
        Logs.Init("RTX Video Super Resolution extension loaded");
        RTXVSRGroup = new("RTX Video Super Resolution", Toggles: false, Open: false, IsAdvanced: false);

        ScaleParam = T2IParamTypes.Register<float>(new("RTX VSR Scale", "Scale factor for RTX Video Super Resolution (1.0-4.0). Higher values increase output resolution.",
            "2.0", Toggleable: true, Group: RTXVSRGroup, FeatureFlag: "comfyui", Min: 1.0f, Max: 4.0f, Step: 0.01f));

        QualityParam = T2IParamTypes.Register<string>(new("RTX VSR Quality", "Quality level for RTX Video Super Resolution. Higher quality uses more VRAM and is slower.",
            "ULTRA", Toggleable: true, Group: RTXVSRGroup, FeatureFlag: "comfyui",
            GetValues: (_) => ["LOW", "MEDIUM", "HIGH", "ULTRA"]));

        WorkflowGenerator.AddStep(g =>
        {
            if (g.UserInput.TryGet(ScaleParam, out float scale) && g.UserInput.TryGet(QualityParam, out string quality))
            {
                string rtxNode = g.CreateNode("RTXVideoSuperResolution", new JObject()
                {
                    ["images"] = g.CurrentMedia.Path,
                    ["scale"] = scale,
                    ["quality"] = quality
                });
                g.CurrentMedia = g.CurrentMedia.WithPath([rtxNode, 0]);
            }
        }, 9);
    }
}
