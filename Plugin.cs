using BepInEx;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using GorillaLocomotion;
using System.IO;

namespace MirrorGun
{
    [BepInPlugin(PluginInfo.GUID, PluginInfo.Name, PluginInfo.Version)]

    public class Plugin : BaseUnityPlugin
    {
        private static GameObject circle = null;

        void OnEnable()
        {
            HarmonyPatches.ApplyHarmonyPatches();
        }

        void OnDisable()
        {
            HarmonyPatches.RemoveHarmonyPatches();
        }
        public static Color purple
        {
            get
            {
                return new Color(0.61f, 0.25f, 0.98f);
            }
        }
        void Update()
        {
            try
            {
                bool flag = false;
                bool flag2 = false;
                List<InputDevice> list = new List<InputDevice>();
                InputDevices.GetDevices(list);
                InputDevices.GetDevicesWithCharacteristics((InputDeviceCharacteristics)512, list);

                if (list.Count > 0)
                {
                    list[0].TryGetFeatureValue(CommonUsages.triggerButton, out flag);
                    list[0].TryGetFeatureValue(CommonUsages.gripButton, out flag2);
                }
                bool flag3 = flag2;
                if (flag3)
                {
                    RaycastHit raycastHit;
                    bool flag4 = Physics.Raycast(GorillaLocomotion.Player.Instance.rightControllerTransform.position - GorillaLocomotion.Player.Instance.rightControllerTransform.up, -GorillaLocomotion.Player.Instance.rightControllerTransform.up, out raycastHit) && circle == null;
                    if (flag4)
                    {
                        circle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                        Destroy(circle.GetComponent<Rigidbody>());
                        Destroy(circle.GetComponent<SphereCollider>());
                        circle.GetComponent<Renderer>().material.color = purple;
                        circle.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
                    }
                    circle.transform.position = raycastHit.point;
                    bool flag5 = flag;
                    if (flag5)
                    {
                        GameObject mirror = GameObject.Find("Level/lower level/mirror (1)");
                        mirror.SetActive(true);

                        GameObject mirrorStand = GameObject.Find("Level/lower level/mirror (1)/board");
                        Destroy(mirrorStand.GetComponent<MeshCollider>());

                        GameObject mirrorThangy = GameObject.Find("Level/lower level/mirror (1)/stand");
                        Destroy(mirrorThangy.GetComponent<MeshCollider>());

                        if (mirror == null)
                        {
                            Debug.Log("Mirror object not found");
                        }
                        else
                        {
                            mirror.transform.position = circle.transform.position;
                            mirror.transform.LookAt(Player.Instance.bodyCollider.transform);
                        }
                    }
                }
                else
                {
                    UnityEngine.Object.Destroy(circle);
                }
            }
            catch (Exception ex)
            {
                File.WriteAllText("MirrorGunErrors.log", ex.ToString());
                File.WriteAllText("MirrorGun.log", "Yay!");
            }
        }
    }
}
