using System.Collections.Generic;
using ITCafe.Data.Items;
using UnityEngine;
using UnityEngine.Localization.Tables;

namespace ITCafe.Data
{
    public interface IMissionSetup
    {
        public GameObject SceneSetupPrefab { get; }
        public IItemInfoCollection ItemInfoCollection { get; }
        public IRecipeCollection RecipeCollection { get; }
        public IGuideData GuideData { get; }
        public IMissionEvaluation MissionEvaluation { get; }
        public ILocaleTableCollection LocaleTableCollection { get; }
    }
}