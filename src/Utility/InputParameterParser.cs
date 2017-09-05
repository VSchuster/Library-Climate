//  Copyright 2007-2010 Portland State University, University of Wisconsin-Madison
//  Author: Robert Scheller, Ben Sulman

using Landis.Core;
using Landis.SpatialModeling;
using Edu.Wisc.Forest.Flel.Util;
//using Landis.Library.Succession;
using System.Collections.Generic;
using System;

namespace Landis.Library.Climate
{
    /// <summary>
    /// A parser that reads biomass succession parameters from text input.
    /// </summary>
    public class InputParametersParser
        : TextParser<IInputParameters>
    {
        private string landisDataValue;

        public override string LandisDataValue
        {
            get
            {
                return landisDataValue;  //"Climate Config";
            }
        }


        public static class Names
        {
            //public const string Timestep = "Timestep";            
            public const string LandisData = "LandisData";
            public const string ClimateConfigFile = "ClimateConfigFile";
            public const string ClimateTimeSeries = "ClimateTimeSeries";
            public const string ClimateFile = "ClimateFile";
            public const string ClimateFileFormat = "ClimateFileFormat";
            public const string SpinUpClimateTimeSeries = "SpinUpClimateTimeSeries";
            public const string SpinUpClimateFile = "SpinUpClimateFile";
            public const string SpinUpClimateFileFormat = "SpinUpClimateFileFormat";
            // Added for Fire Climate Output
            public const string FireClimate = "UsingFireClimate";
            public const string RHSlopeAdjust = "RelativeHumiditySlopeAdjust";
            public const string SpringStart = "SpringStart";
            public const string WinterStart = "WinterStart";
        }

        //---------------------------------------------------------------------

        static InputParametersParser()
        {
        }

        //---------------------------------------------------------------------

        public InputParametersParser()
        {
            this.landisDataValue = "Climate Config";
        }

        //---------------------------------------------------------------------

        protected override IInputParameters Parse()
        {
            InputVar<string> landisData = new InputVar<string>("LandisData");
            ReadVar(landisData);
            if (landisData.Value.Actual != "Climate Config")
                throw new InputValueException(landisData.Value.String, "The value is not \"{0}\"", "Climate Config");

            InputParameters parameters = new InputParameters();

            string climateTimeSeries_PossibleValues = "Monthly_AverageAllYears, Monthly_AverageWithVariation, Monthly_RandomYears, Daily_RandomYears, Daily_AverageAllYears, Daily_SequencedYears, Monthly_SequencedYears";
            string climateFileFormat_PossibleValues = "Daily_Temp-C_Precip-mmDay, Monthly_Temp-C_Precip-mmMonth, Daily_Temp-K_Precip-kgM2Sec, Monthly_Temp-K_Precip-kgM2Sec, mauer_daily, monthly_temp-k_precip-mmmonth, daily_temp-k_precip-mmday";

            //InputVar<string> climateConfigFile = new InputVar<string>(Names.ClimateConfigFile);
            //ReadVar(climateConfigFile);
            //parameters.ClimateConfigFile = climateConfigFile.Value;

            InputVar<string> climateTimeSeries = new InputVar<string>(Names.ClimateTimeSeries);
            ReadVar(climateTimeSeries);
            parameters.ClimateTimeSeries = climateTimeSeries.Value;

            InputVar<string> climateFile = new InputVar<string>(Names.ClimateFile);
            ReadVar(climateFile);
            parameters.ClimateFile = climateFile.Value;

            InputVar<string> climateFileFormat = new InputVar<string>(Names.ClimateFileFormat);
            ReadVar(climateFileFormat);
            parameters.ClimateFileFormat = climateFileFormat.Value;

            InputVar<string> spinUpClimateTimeSeries = new InputVar<string>(Names.SpinUpClimateTimeSeries);
            ReadVar(spinUpClimateTimeSeries);
            parameters.SpinUpClimateTimeSeries = spinUpClimateTimeSeries.Value;

            InputVar<string> spinUpClimateFile = new InputVar<string>(Names.SpinUpClimateFile);
            InputVar<string> spinUpClimateFileFormat = new InputVar<string>(Names.SpinUpClimateFileFormat);

            ReadVar(spinUpClimateFile);
            parameters.SpinUpClimateFile = spinUpClimateFile.Value;

            ReadVar(spinUpClimateFileFormat);
            parameters.SpinUpClimateFileFormat = spinUpClimateFileFormat.Value;

            if (!climateTimeSeries_PossibleValues.ToLower().Contains(parameters.ClimateTimeSeries.ToLower()) || !climateTimeSeries_PossibleValues.ToLower().Contains(parameters.SpinUpClimateTimeSeries.ToLower()))
            {
                throw new ApplicationException("Error in parsing climate-generator input file: invalid value for ClimateTimeSeries or SpinupTimeSeries provided. Possible values are: " + climateTimeSeries_PossibleValues);
            }

            if (!climateFileFormat_PossibleValues.ToLower().Contains(parameters.ClimateFileFormat.ToLower()) || !climateFileFormat_PossibleValues.ToLower().Contains(parameters.SpinUpClimateFileFormat.ToLower()))
            {
                throw new ApplicationException("Error in parsing climate-generator input file: invalid value for File Format provided. Possible values are: " + climateFileFormat_PossibleValues);
            }

            if (parameters.ClimateTimeSeries.ToLower().Contains("daily") && !parameters.ClimateFileFormat.ToLower().Contains("daily"))
            {
                throw new ApplicationException("You are requesting a Daily Time Step but not inputting daily data:" + parameters.ClimateTimeSeries + " and " + parameters.ClimateFileFormat);
            }

            InputVar<string> fireClimate = new InputVar<string>(Names.FireClimate);
            if (ReadOptionalVar(fireClimate))
            {
                string usingFireClimate = fireClimate.Value;
                usingFireClimate.ToLower();
                if (usingFireClimate == "yes")
                    parameters.FireClimate = true;
                else if (usingFireClimate == "no")
                {
                    parameters.FireClimate = false;
                }
                else
                {
                    throw new ApplicationException(System.String.Format("UsingFireClimate variable given: {0} \n\t UsingFireClimate must be \"yes\" or \"no\"", usingFireClimate));
                }
            }
            else
            {
                parameters.FireClimate = false;
            }
            
            // Optional Vars required for Fire Weather Index calculation
            if (parameters.FireClimate)
            {
                InputVar<double> rHSlopeAdjust = new InputVar<double>(Names.RHSlopeAdjust);
                ReadVar(rHSlopeAdjust);
                parameters.RHSlopeAdjust = rHSlopeAdjust.Value;

                // ----Optional vars for Fire Climate Output ----

                // Julian day = 60 if no value is given
                InputVar<int> springStart = new InputVar<int>(Names.SpringStart);
                if (ReadOptionalVar(springStart))
                {
                    parameters.SpringStart = springStart.Value;
                }
                else
                {
                    parameters.SpringStart = 60;
                }
                // Julian day = 336 if no value is given
                InputVar<int> winterStart = new InputVar<int>(Names.WinterStart);
                if (ReadOptionalVar(winterStart))
                {

                    parameters.WinterStart = winterStart.Value;
                }
                else
                {
                    parameters.WinterStart = 336;
                }


                // Verification checks for proper parameter inputs for Fire Climate
                // VS: clarify with alec min(RHSlopeAdjust)
                if (parameters.RHSlopeAdjust < 0)
                {
                    throw new ApplicationException(System.String.Format("RHSlopeAdjust must be greater than 0"));
                }

                if (parameters.WinterStart <= 0 || parameters.WinterStart > 365)
                {
                    throw new ApplicationException(System.String.Format("Winter Start date must be between 0 and 365"));
                }

                if (parameters.SpringStart <= 0 || parameters.SpringStart > 365)
                {
                    throw new ApplicationException(System.String.Format("Spring Start date must be between 0 and 365"));
                }

                if (parameters.SpringStart >= parameters.WinterStart)
                {
                    throw new ApplicationException(System.String.Format("Spring start day: {0}, cannot be later or the same day as Winter start day: {1}", parameters.SpringStart, parameters.WinterStart));
                }


            }


            return parameters; 


        }
         //---------------------------------------------------------------------

//        public static TimeSeriesNames TimeSeriesParse(string word)
//        {
//Monthly_AverageAllYears, Monthly_AverageWithVariation, Monthly_RandomYear, Monthly_SequencedYears, Daily_RandomYear, Daily_AverageAllYears, Daily_SequencedYears
            
//            if (word == "gamma")
//                return Distribution.gamma;
//            else if (word == "lognormal")
//                return Distribution.lognormal;
//            else if (word == "normal")
//                return Distribution.normal;
//            else if (word == "Weibull")
//                return Distribution.Weibull;
//            throw new System.FormatException("Valid Distributions: gamma, lognormal, normal, Weibull");
//        }   
    }




}
