using UnityEditor;
using UnityEngine;
using UnityEditor.Build.Reporting;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;

public class BuildWindowsCustom : MonoBehaviour
{
    [MenuItem("Build/Build ALL")]
    public static void MyBuild()
    {
        string buildLog = "--LOG--";

        // Build player.
        BuildPlayerOptions options = new BuildPlayerOptions();
        options.target = BuildTarget.StandaloneWindows;
        options.options = BuildOptions.None;

        List<string> scenesString = new List<string>();

        foreach(EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            scenesString.Add(scene.path);
        }
        options.scenes = scenesString.ToArray();
        

        options.locationPathName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\windows\\Element Quest.exe";

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows);
        BuildReport report = BuildPipeline.BuildPlayer(options);


        buildLog += report.summary.platform + " | " + report.summary.result.ToString() + ". ";


        Process proc = new Process();
        proc.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\devtoolswin\\self.bat";
        proc.Start();



        //MACOS
        options.target = BuildTarget.StandaloneOSX;
        options.locationPathName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\macos\\Element Quest.exe";

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneOSX);
        report = BuildPipeline.BuildPlayer(options);


        buildLog+=report.summary.platform + " | " + report.summary.result.ToString() +". ";


        Process proc2 = new Process();
        proc2.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\devtoolsmac\\self.bat";
        proc2.Start();


        //linux
        options.target = BuildTarget.StandaloneLinux64;
        options.locationPathName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\linux\\Element Quest.x86_64";

        EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTargetGroup.Standalone, BuildTarget.StandaloneLinux64);
        report = BuildPipeline.BuildPlayer(options);

        buildLog += report.summary.platform + " | " + report.summary.result.ToString() + ". ";



        Process proc3 = new Process();
        proc3.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\content\\devtoolslinux\\self.bat";
        proc3.Start();


        UnityEngine.Debug.Log(buildLog);

        Process proc4 = new Process();
        proc4.StartInfo.FileName = Directory.GetCurrentDirectory() + "\\element_quest_sdk\\tools\\ContentBuilder\\run_build.bat";
        proc4.Start();

        UnityEngine.Debug.Log("Build Complete! "+ proc4.ProcessName);

    }
}