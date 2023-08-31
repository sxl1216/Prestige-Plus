﻿using BlueprintCore.Utils;
using JetBrains.Annotations;
using Kingmaker.Blueprints;
using Kingmaker.EntitySystem.Entities;
using Kingmaker.RuleSystem.Rules.Abilities;
using Kingmaker.RuleSystem;
using Kingmaker.UnitLogic.Abilities;
using Kingmaker.UnitLogic.Class.Kineticist;
using Kingmaker.UnitLogic.Mechanics;
using Kingmaker;
using Kingmaker.UnitLogic.Mechanics.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kingmaker.EntitySystem.Stats;
using Kingmaker.UnitLogic.Mechanics.Properties;
using BlueprintCore.Blueprints.References;
using BlueprintCore.Utils.Types;
using PrestigePlus.PrestigeClasses;
using Kingmaker.Blueprints.JsonSystem;

namespace PrestigePlus.Modify
{
    [TypeId("{1826B09B-766F-46C0-B964-9A231BAD1E81}")]
    internal class CustomDC : ContextCalculateAbilityParams
    {
        private static readonly LogWrapper Logger = LogWrapper.Get("PrestigePlus");
        private static readonly string ArchetypeGuid = "{A9827D49-8599-4525-B763-0E4554DCC1A0}";
        public override AbilityParams Calculate(MechanicsContext context)
        {
            UnitEntityData maybeCaster = context.MaybeCaster;
            if (maybeCaster == null)
            {
                PFLog.Default.Error(this, "Caster is missing", Array.Empty<object>());
                return context.Params;
            }
            BlueprintScriptableObject associatedBlueprint = context.AssociatedBlueprint;
            UnitEntityData caster = maybeCaster;
            AbilityExecutionContext sourceAbilityContext = context.SourceAbilityContext;
            return this.MyCalculate(context, associatedBlueprint, caster, (sourceAbilityContext != null) ? sourceAbilityContext.Ability : null);
        }

        private AbilityParams MyCalculate([CanBeNull] MechanicsContext context, [NotNull] BlueprintScriptableObject blueprint, [NotNull] UnitEntityData caster, [CanBeNull] AbilityData ability)
        {
            StatType value = StatType.Charisma;
            var level = ContextValues.Rank(Kingmaker.Enums.AbilityRankType.DamageBonus);
            RuleCalculateAbilityParams ruleCalculateAbilityParams = (ability != null) ? new RuleCalculateAbilityParams(caster, ability) : new RuleCalculateAbilityParams(caster, blueprint, null);
            ruleCalculateAbilityParams.ReplaceStat = new StatType?(value);
            if (this.StatTypeFromCustomProperty)
            {
                ruleCalculateAbilityParams.ReplaceStatBonusModifier = new int?(this.m_CustomProperty.Get().GetInt(caster));
            }
            var guid = "{A9827D49-8599-4525-B763-0E4554DCC1A0}";
            var archetype = BlueprintTool.GetRef<BlueprintCharacterClassReference>(guid);
            ruleCalculateAbilityParams.ReplaceCasterLevel = new int?(caster.Descriptor.Progression.GetClassLevel(archetype));
            ruleCalculateAbilityParams.ReplaceSpellLevel = new int?(caster.Descriptor.Progression.GetClassLevel(archetype));
            if (context != null)
            {
                return context.TriggerRule<RuleCalculateAbilityParams>(ruleCalculateAbilityParams).Result;
            }
            return Rulebook.Trigger<RuleCalculateAbilityParams>(ruleCalculateAbilityParams).Result;
        }

    }
}
