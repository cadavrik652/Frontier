using Content.Shared.Humanoid.Prototypes;
using Content.Shared.Dataset;
using System.Text.RegularExpressions;
using Robust.Shared.Random;
using Robust.Shared.Prototypes;
using Robust.Shared.Enums;

namespace Content.Shared.Humanoid
{
    /// <summary>
    /// Figure out how to name a humanoid with these extensions.
    /// </summary>
    public sealed class NamingSystem : EntitySystem
    {
        [Dependency] private readonly IRobustRandom _random = default!;
        [Dependency] private readonly IPrototypeManager _prototypeManager = default!;

        public string GetName(string species, Gender? gender = null)
        {
            // if they have an old species or whatever just fall back to human I guess?
            // Some downstream is probably gonna have this eventually but then they can deal with fallbacks.
            if (!_prototypeManager.TryIndex(species, out SpeciesPrototype? speciesProto))
            {
                speciesProto = _prototypeManager.Index<SpeciesPrototype>("Human");
                Log.Warning($"Unable to find species {species} for name, falling back to Human");
            }

            switch (speciesProto.Naming)
            {
                case SpeciesNaming.First:
                    return Loc.GetString("namepreset-first",
                        ("first", GetFirstName(speciesProto, gender)));
                case SpeciesNaming.TajaranGenerator:
                    return GetTajaranName(speciesProto, gender);
                // Start of Nyano - Summary: for Oni naming
                case SpeciesNaming.LastNoFirst:
                    return Loc.GetString("namepreset-lastnofirst",
                        ("first", GetFirstName(speciesProto, gender)), ("last", GetLastName(speciesProto)));
                // End of Nyano - Summary: for Oni naming
                case SpeciesNaming.TheFirstofLast:
                    return Loc.GetString("namepreset-thefirstoflast",
                        ("first", GetFirstName(speciesProto, gender)), ("last", GetLastName(speciesProto)));
                case SpeciesNaming.FirstDashFirst:
                    return Loc.GetString("namepreset-firstdashfirst",
                        ("first1", GetFirstName(speciesProto, gender)), ("first2", GetFirstName(speciesProto, gender)));
                case SpeciesNaming.LastFirst: // DeltaV: Rodentia name scheme
                    return Loc.GetString("namepreset-lastfirst",
                        ("last", GetLastName(speciesProto)), ("first", GetFirstName(speciesProto, gender)));
                case SpeciesNaming.FirstLast:
                default:
                    return Loc.GetString("namepreset-firstlast",
                        ("first", GetFirstName(speciesProto, gender)), ("last", GetLastName(speciesProto)));
            }
        }

        public string GetFirstName(SpeciesPrototype speciesProto, Gender? gender = null)
        {
            switch (gender)
            {
                case Gender.Male:
                    return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.MaleFirstNames).Values);
                case Gender.Female:
                    return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleFirstNames).Values);
                default:
                    if (_random.Prob(0.5f))
                        return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.MaleFirstNames).Values);
                    else
                        return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleFirstNames).Values);
            }
        }

        // Corvax-LastnameGender-Start: Added custom gender split logic
        public string GetLastName(SpeciesPrototype speciesProto, Gender? gender = null)
        {
            switch (gender)
            {
                case Gender.Male:
                    return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.MaleLastNames).Values);
                case Gender.Female:
                    return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleLastNames).Values);
                default:
                    if (_random.Prob(0.5f))
                        return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.MaleLastNames).Values);
                    else
                        return _random.Pick(_prototypeManager.Index<DatasetPrototype>(speciesProto.FemaleLastNames).Values);
            }
        }
        // Corvax-LastnameGender-End

    /// <summary>
    /// #TODOLIST add all species name generators and make female endings support
    /// </summary>
        public string GetTajaranName(SpeciesPrototype speciesProto, Gender? gender = null)
        {
            List<string> _ru_names_syllables = new List<string> { "кан","тай","кир","раи","кии","мир","кра","тэк","нал","вар","хар","марр","ран","дарр",
	            "мирк","ири","дин","манг","рик","зар","раз","кель","шера","тар","кей","ар","но","маи","зир","кер","нир","ра",
	            "ми","рир","сей","эка","гир","ари","нэй","нре","ак","таир","эрай","жин","мра","зур","рин","сар","кин","рид","эра","ри","эна"};
            string _apostrophe =  "'";
            string _newName = "";
            string _fullName = "";

            for (int i = 0; i<2; i++)
            {
                for (int x = _random.Next(1,2); x>0; x--) _newName += _random.PickAndTake(_ru_names_syllables);
                _newName += _apostrophe;
                _apostrophe = "";
            }
            _fullName= string.Concat(char.ToUpper(_newName[0]).ToString(), _newName.Remove(0, 1));
            if (_random.Prob(0.75f)) _fullName += " " +  _random.Pick(new List<string> {"Хадии","Кайтам","Жан-Хазан","Нъярир’Ахан"});
            else if (_random.Prob(0.8f))  _fullName += " " +  _random.Pick(new List<string> {"Энай-Сэндай","Наварр-Сэндай","Року-Сэндай","Шенуар-Сэндай"});
            return _fullName;
        }
    }
}
