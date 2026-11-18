using System;
using Il2CppSystem.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Logger = APNestClient.ModLoader.Logger;
using Object = UnityEngine.Object;

namespace APNestClient;

public static class TertiaryTeleprinter
{
    // register the clone as it's own printer (so no overlap with one of the others occurs
    public const Teleprinter.Teleprinters Type = (Teleprinter.Teleprinters)2;

    private const string PaperName = "Text Logger Collider";

    // hand-picked spot to the left of the secondary 
    private static readonly Vector3 WallPoint = new Vector3(3.617f, -0.266f, -22.134f);
    private static readonly Vector3 WallNormal = new Vector3(-1f, 0f, 0f);
    private const float WallOffset = 0.12f;
    private const float TextYaw = 90f + 30f; // small rotation to make it more readable
    private static readonly Vector3 WallSlide = new Vector3(0f, 0f, 0.15f);

    // 'paper' behind the text (spoiler: it's just an off-white rectangle
    private const float PaperThickness = 0.002f;
    private const float PaperGap = 0.005f;
    private const float PaperExtraWidth = 0.4f; // fine tune width for AP messages
    private const float PaperMargin = 0.03f;
    private static readonly Color PaperColor = new Color(0.92f, 0.90f, 0.82f, 1f);

    private static Teleprinter _printer;
    private static Transform _rig;
    private static Transform _orders;
    private static int _lastAttemptGeneration = -1;

    public static Teleprinter Printer => _printer != null ? _printer : null;

    public static void Tick()
    {
        if (_printer != null || _lastAttemptGeneration == MissionReadyGuard.MissionGeneration)
        {
            return;
        }

        if (!MissionReadyGuard.IsReady())
        {
            return;
        }

        // one attempt per mission, so a failure does not retry every frame
        _lastAttemptGeneration = MissionReadyGuard.MissionGeneration;

        try
        {
            Create();
        }
        catch (Exception e)
        {
            Logger.Error("Tertiary teleprinter creation failed: " + e);
        }
    }

    public static void Print(List<string> lines)
    {
        if (Printer == null)
        {
            Logger.Warning("Tertiary teleprinter not available, dropping " + lines.Count + " lines");
            return;
        }

        Printer.SubmitLines(Il2CppSystem.Guid.NewGuid().ToString(), lines.Cast<IEnumerable<string>>(), null, false);
    }

    private static void Create()
    {
        Teleprinter source = Teleprinter.GetTeleprinter(Teleprinter.Teleprinters.Secondary);
        if (source == null)
        {
            Logger.Warning("Tertiary teleprinter: Secondary teleprinter not found");
            return;
        }

        // the Secondary's "Teleprinter Radio"
        Transform radio = source.transform.parent;

        // inactive rig so the clone's Awake (which registers into the lookup by
        // TeleprinterType) only runs after I changed the type
        GameObject rig = new GameObject("[AP Teleprinter]");
        rig.SetActive(false);
        // parent under the printers' scene root so the clones are unloaded together with them
        rig.transform.SetParent(source.transform.root, false);

        GameObject clone = CloneIntoRig(source.gameObject, radio, rig.transform);
        clone.name = "AP Orders"; // Orders is the moving paper
        Vector3 initial = source._initialPaperLocalPos;
        if (initial != Vector3.zero)
        {
            clone.transform.localPosition = initial;
        }

        Transform paper = radio.Find(PaperName);
        if (paper != null)
        {
            GameObject paperClone = CloneIntoRig(paper.gameObject, radio, rig.transform);
            paperClone.name = "AP Paper";
            StripToVisuals(paperClone);
            MakePaper(paperClone, clone.transform);
            FitTextToPaper(clone.transform, paperClone.transform);
        }
        else
        {
            Logger.Warning("Tertiary teleprinter: no '" + PaperName + "' under " + radio.name + ", no paper");
        }

        _rig = rig.transform;
        _orders = clone.transform;
        PlaceRig(WallPoint + WallNormal * WallOffset + WallSlide, Quaternion.Euler(0f, TextYaw, 0f));

        Teleprinter printer = clone.GetComponent<Teleprinter>();
        printer.TeleprinterType = Type;

        // persistent listeners point at the Secondary's lamps
        DisablePersistent(printer.onTypingStarted);
        DisablePersistent(printer.onAllJobsCompleted);
        DisablePersistent(printer.onJobStarted);
        DisablePersistent(printer.onJobCompleted);
        DisablePersistent(printer.onCharacterPrinted);
        DisablePersistent(printer.onLineFeed);
        DisablePersistent(printer.onJobsEnqueued);
        DisablePersistent(printer.OnSignal_Reset);
        DisablePersistent(printer.OnSignal_None);
        DisablePersistent(printer.OnSignal_High);
        DisablePersistent(printer.OnSignal_Low);
        DisablePersistent(printer.OnSignal_Success);
        DisablePersistent(printer.onCharacterPrintedDetailed);
        DisablePersistent(printer.onLineTransition);

        TeleprinterLineRangeSelector3D selector = clone.GetComponent<TeleprinterLineRangeSelector3D>();
        if (selector != null)
        {
            Object.DestroyImmediate(selector);
        }

        // don't start out with a copy of whatever the Secondary has printed so far
        TMP_Text text = clone.GetComponent<TMP_Text>();
        if (text != null)
        {
            text.text = "";
            text.richText = true; // for the fancy colors
            DisableStencilMask(text);
        }

        rig.SetActive(true);
        printer.ClearAll();
        _printer = printer;

        List<string> test = new List<string>();
        test.Add("Connection to Supply Company 'Archipelago' established.");
        Print(test);
    }

    private static void PlaceRig(Vector3 textPosition, Quaternion textRotation)
    {
        Quaternion rigRotation = textRotation * Quaternion.Inverse(_orders.localRotation);
        _rig.SetPositionAndRotation(textPosition - rigRotation * _orders.localPosition, rigRotation);
    }

    private static GameObject CloneIntoRig(GameObject original, Transform frame, Transform rig)
    {
        Transform o = original.transform;
        GameObject clone = Object.Instantiate(original, rig);
        clone.transform.localPosition = frame.InverseTransformPoint(o.position);
        clone.transform.localRotation = Quaternion.Inverse(frame.rotation) * o.rotation;
        clone.transform.localScale = o.localScale;
        return clone;
    }

    private static void StripToVisuals(GameObject go)
    {
        foreach (Transform child in go.GetComponentsInChildren<Transform>(true))
        {
            Il2CppInterop.Runtime.InteropTypes.Arrays.Il2CppArrayBase<Component> components = child.GetComponents<Component>();
            // reverse, so components are removed before the ones they depend on (RequireComponent)
            for (int i = components.Length - 1; i >= 0; i--)
            {
                Component c = components[i];
                if (c == null || c.TryCast<Transform>() != null || c.TryCast<MeshFilter>() != null || c.TryCast<MeshRenderer>() != null)
                {
                    continue;
                }
                Object.DestroyImmediate(c);
            }
        }
    }

    private static void MakePaper(GameObject paper, Transform orders)
    {
        Transform t = paper.transform;
        Vector3 scale = t.localScale;
        Vector3 pos = t.localPosition;

        float left = pos.x + scale.x / 2f;
        float width = scale.x + PaperExtraWidth;

        t.localScale = new Vector3(width, scale.y, PaperThickness);
        t.localPosition = new Vector3(left - width / 2f, pos.y, orders.localPosition.z - PaperGap - PaperThickness / 2f);

        MeshRenderer r = paper.GetComponent<MeshRenderer>();
        if (r == null)
        {
            Logger.Warning("Tertiary teleprinter: paper has no renderer");
            return;
        }

        Material m = new Material(r.sharedMaterial);
        m.name = "AP Paper";
        if (m.HasProperty("_BaseColor"))
        {
            m.SetColor("_BaseColor", PaperColor);
        }
        r.sharedMaterial = m;
        r.enabled = true;
    }
    
    private static void FitTextToPaper(Transform orders, Transform paper)
    {
        RectTransform rect = orders.TryCast<RectTransform>();
        if (rect == null)
        {
            Logger.Warning("Tertiary teleprinter: Orders has no RectTransform, cannot fit text");
            return;
        }

        float paperWidth = paper.localScale.x;
        float paperLeft = paper.localPosition.x + paperWidth / 2f;
        float width = paperWidth - 2f * PaperMargin;

        rect.sizeDelta = new Vector2(width, rect.sizeDelta.y);

        TMP_Text tmp = orders.GetComponent<TMP_Text>();
        if (tmp != null)
        {
            tmp.margin = Vector4.zero;
            tmp.enableWordWrapping = true;
        }

        Vector3 pos = orders.localPosition;
        orders.localPosition = new Vector3(paperLeft - PaperMargin - rect.pivot.x * width, pos.y, pos.z);
    }

    private const float StencilCompareAlways = 8f;

    private static void DisableStencilMask(TMP_Text text)
    {
        Material shared = text.fontSharedMaterial;
        if (shared == null || !shared.HasProperty("_StencilComp"))
        {
            Logger.Warning("Tertiary teleprinter: font material has no stencil, leaving it as is");
            return;
        }

        Material unmasked = new Material(shared);
        unmasked.name = shared.name + " (AP unmasked)";
        unmasked.SetFloat("_StencilComp", StencilCompareAlways);
        text.fontSharedMaterial = unmasked;
    }

    private static void DisablePersistent(UnityEventBase ev)
    {
        if (ev == null)
        {
            return;
        }

        for (int i = 0; i < ev.GetPersistentEventCount(); i++)
        {
            ev.SetPersistentListenerState(i, UnityEventCallState.Off);
        }
    }
}
