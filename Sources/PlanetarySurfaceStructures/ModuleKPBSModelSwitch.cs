﻿using UnityEngine;
using System.Collections.Generic;

namespace PlanetarySurfaceStructures
{
    class ModuleKPBSModelSwitch : PartModule
    {

        //the names of the transforms
        [KSPField]
        public string transformNames = string.Empty;

        [KSPField]
        public string transformVisibleNames = string.Empty;

        //--------------persistent states----------------
        [KSPField(isPersistant = true)]
        public int numModel = 0;

        //the list of models
        List<ModelTransforms> models;

        List<string> visibleNames;

        public bool initialized = false;

        //the part that is enabled and disabled
        public override void OnStart(StartState state)
        {
            base.OnStart(state);

            string[] transformGroupNames = transformNames.Split(',');
            string[] transformGroupVisibleNames = transformVisibleNames.Split(',');
            models = new List<ModelTransforms>();
            visibleNames = new List<string>();

            //----------------------------------------------------------
            //create the list of transforms to be made switchable
            //----------------------------------------------------------
            for (int k = 0; k < transformGroupNames.Length; k++)
            {
                name = transformGroupNames[k].Trim();

                List<Transform> transforms = new List<Transform>();
                transforms.AddRange(part.FindModelTransforms(name));

                ModelTransforms mt = new ModelTransforms();
                mt.transforms = new List<Transform>();
                mt.transforms.AddRange(transforms);

                models.Add(mt);
                if (transformGroupVisibleNames.Length == transformGroupNames.Length) {
                    visibleNames.Add(transformGroupVisibleNames[k]);
                }
                else
                {
                    visibleNames.Add(transformGroupNames[k]);
                }
            }

            //when there is only one model, we do not need to show the controls
            if (models.Count < 2)
            {
                Events["toggleModel"].active = false;
                Events["toggleModelNext"].active = false;
                Events["toggleModelPrev"].active = false;
            }
            //when there are two models make the controls appear as a switch between two parts
            else if (models.Count == 2)
            {
                Events["toggleModel"].active = true;
                Events["toggleModel"].guiName = "Switch to: " + getName(numModel + 1);
                Events["toggleModelNext"].active = false;
                Events["toggleModelPrev"].active = false;
            }
            //when there are more than two models, let the user iterate over them
            else if (models.Count > 2)
            {
                Events["toggleModel"].active = false;
                Events["toggleModelNext"].active = true;
                Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
                Events["toggleModelPrev"].active = true;
                Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            }
            


            if (!HighLogic.LoadedSceneIsEditor)
            {
                Events["toggleModel"].guiActive = false;
            }
            updateActiveModel();
        }

        /**
         * The update method of the module
         */
        public void Update()
        {
            if (!initialized)
            {
                updateActiveModel();
                initialized = true;
            }
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModel", guiName = "Switch Model", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModel()
        {
            numModel++;
            if (numModel >= models.Count)
            {
                numModel = 0;
            }
            Events["toggleModel"].guiName = "Switch to: " + getName(numModel + 1);
            updateActiveModel();
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModelNext", guiName = "Next: ", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModelNext()
        {
            numModel++;
            if (numModel >= models.Count)
            {
                numModel = 0;
            }
            Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
            Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            updateActiveModel();
        }

        /**
         * Toggle the visible part
         */
        [KSPEvent(name = "toggleModelPrev", guiName = "Previous: ", guiActive = true, guiActiveUnfocused = false, unfocusedRange = 5f, guiActiveEditor = true)]
        public void toggleModelPrev()
        {
            numModel--;
            if (numModel < 0)
            {
                numModel = models.Count-1;
            }
            Events["toggleModelNext"].guiName = "Next: " + getName(numModel + 1);
            Events["toggleModelPrev"].guiName = "Previous: " + getName(numModel - 1);
            updateActiveModel();
        }


        // Get the name of the visible part
        private string getName(int index)
        {
            if ((visibleNames.Count > 0) && (visibleNames.Count == models.Count))
            {
                if (index < 0)
                {
                    index += visibleNames.Count;
                }
                else if (index >= models.Count)
                {
                    index -= visibleNames.Count;
                }

                return visibleNames[index];
            }
            return "";
        }


        // Update which model are active or inactive
        private void updateActiveModel()
        {
            for (int i = 0; i < models.Count; i++)
            {
                for (int j = 0; j < models[i].transforms.Count; j++)
                {
                    if (i == numModel)
                    {
                        models[i].transforms[j].gameObject.SetActive(true);
                    }
                    else
                    {
                        models[i].transforms[j].gameObject.SetActive(false);
                    }
                }
            }
        }

        // An internal struct that holds the data for the switchable parts
        private class ModelTransforms
        {
            public List<Transform> transforms;
        }
    }
}
