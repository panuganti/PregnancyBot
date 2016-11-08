using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PregnancyLibrary
{
    [DataContract]
    public class User
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public DateTime StartTime { get; set; }
        [DataMember]
        public string Name { get; set; }
        [DataMember]
        public DateTime LMPDate { get; set; }
    }

    [DataContract]
    public class BotToUserMilestones
    {
        [DataMember]
        public BotToUserMilestonesTypes Type { get; set; }
        [DataMember]
        public string UserId { get; set; }
        [DataMember]
        public DateTime DateMilestoneRecorded { get; set; }
    }

    [DataContract]
    public enum BotToUserMilestonesTypes
    {
        [EnumMember]
        LMP,
        [EnumMember]
        Introduction
    }

    [DataContract]
    public class Symptom
    {
        [DataMember]
        public string name { get; set; }

        [DataMember]
        public Symptomtype type { get; set; }
        [DataMember]
        public string Text { get; set; }
    }

    public class Remainder
    {
        public DateTime AppointmentDate { get; set; }
    }

    public enum Articles
    {
        Symptoms,
        Baby,
        Body,
        Labor,
        Tracking,
        Planning,
        Medical,
        Nutrition,
        Exercise,
        Lifestyle,
        Recipes
    }

    /*
     * Baby Development Stages
     * Nutrition Tips
     * Body Changes
     * Activity Tips
     */

    /*
     * Place to rmember questions for doctor
     * Baby Name Ideas
     */

    /*
     * DailySummary
     * Milestone
     * Weight
     * Activity
     * Sleep
     * Nutrition
     * Mood
     * Symptoms
     * Medications
     * Blood Pressure
     * Activity 
     * Doctor Appt
     * Notes
     */

    /*
     * Milestones
     * Belly Pic
     * Ultrasound
     * Telling Family & friends
     * Baby's sex
     * Maternity Clothes
     * First Kick
     * Baby Shower
     * Decorating Nursery
     * Cannot See feet
     * Delivery
     */

    public enum Symptomtype
    {
        Common,
        Recent
    }

    /*
    ken burnsken  * 
     * Vitals Info
     * Date Vitals
     * 
     * 
     * 
     * CONVERSATION
     * actual_jsons and response jsons,
     * 
     * INFO_BASE
     * FAQs
     * Date Info
     * 
     * 
     * Info to ask:
     * weight, sleep, moods, activity ...
     * 
     * food symptoms, medications --- food likes, preferences ...
     * 
     */

    /*
     * Symptoms - Suggestions
     * Problematic Symptoms
     */


    /*
     * name
     * birthday
     * height
     * pre-pregnancy weight
     * name ur baby
     * est due date ... entry, lmp (last period), conception, pregnancy duration. ...
     * 
     * 
     * 
     * whats safe to eat ? 
     * 
     * Article database
     * 
     * Questions database
     * 
     * 
     * 
     *  
     * */

    /*
     * Were you pregnant before
     * what you have (diabetes, bp, kidney disease, depression, anxiety, asthma, none)
     * have u been diagnosed (hypertension, gestational diab, placenta previa, clotting disorder, hiv, oligohydramnios, none
     * any medications
     * 
     */
}
