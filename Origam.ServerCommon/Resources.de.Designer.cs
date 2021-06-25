﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.42000
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Origam.ServerCommon {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    public class Resources_de {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resources_de() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Origam.ServerCommon.Resources.de", typeof(Resources_de).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximale Dateigrösse hat ({0} kB) überschritten..
        /// </summary>
        internal static string BlobMaxSizeError {
            get {
                return ResourceManager.GetString("BlobMaxSizeError", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keine Datei ausgewählt..
        /// </summary>
        internal static string BlobNoFileSelected {
            get {
                return ResourceManager.GetString("BlobNoFileSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sie können nicht mehr als 1 Datei auswählen..
        /// </summary>
        internal static string BlobTooManyFilesSelected {
            get {
                return ResourceManager.GetString("BlobTooManyFilesSelected", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Datensatz wurde noch nicht verändert..
        /// </summary>
        internal static string DefaultTooltipNoChange {
            get {
                return ResourceManager.GetString("DefaultTooltipNoChange", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Erstellt von &lt;b&gt;{0}&lt;/b&gt; am &lt;b&gt;{1}&lt;/b&gt;.
        /// </summary>
        internal static string DefaultTooltipRecordCreated {
            get {
                return ResourceManager.GetString("DefaultTooltipRecordCreated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Letztes Update von &lt;b&gt;{0}&lt;/b&gt; am &lt;b&gt;{1}&lt;/b&gt;.
        /// </summary>
        internal static string DefaultTooltipRecordUpdated {
            get {
                return ResourceManager.GetString("DefaultTooltipRecordUpdated", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Maximale Grösse des Anhangs hat ({0} bytes) überschritten..
        /// </summary>
        internal static string ErrorAttachmentMaximumSize {
            get {
                return ResourceManager.GetString("ErrorAttachmentMaximumSize", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fehler beim Laden des Anhangs: Keine Datei angegeben..
        /// </summary>
        internal static string ErrorAttachmentNoFile {
            get {
                return ResourceManager.GetString("ErrorAttachmentNoFile", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anhang nicht verfügbar..
        /// </summary>
        internal static string ErrorAttachmentNotAvailable {
            get {
                return ResourceManager.GetString("ErrorAttachmentNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anhang nicht verfügbar in der Datenbank..
        /// </summary>
        internal static string ErrorAttachmentNotFoundInDb {
            get {
                return ResourceManager.GetString("ErrorAttachmentNotFoundInDb", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anhang nicht vollständig erhalten..
        /// </summary>
        internal static string ErrorAttachmentNotFullyReceived {
            get {
                return ResourceManager.GetString("ErrorAttachmentNotFullyReceived", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fehler beim Aufrufen des Anhangs..
        /// </summary>
        internal static string ErrorAttachmentRetrieval {
            get {
                return ResourceManager.GetString("ErrorAttachmentRetrieval", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Leerer Datensatz, Datei kann nicht heruntergeladen werden..
        /// </summary>
        internal static string ErrorBlobRecordEmpty {
            get {
                return ResourceManager.GetString("ErrorBlobRecordEmpty", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Eingabe nicht gespeichert. Nicht gespeicherte Daten können nicht aktualisert werden..
        /// </summary>
        internal static string ErrorCannotRefreshFormNotSaved {
            get {
                return ResourceManager.GetString("ErrorCannotRefreshFormNotSaved", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auswahl mehrerer Datensätze für ChangeUI wird nicht unterstützt..
        /// </summary>
        internal static string ErrorChangeUIMultipleRecords {
            get {
                return ResourceManager.GetString("ErrorChangeUIMultipleRecords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Versuch, auf die im Datenset nicht vorhandene Reihe `{0}&apos; zuzugreifen..
        /// </summary>
        internal static string ErrorColumnNotFound {
            get {
                return ResourceManager.GetString("ErrorColumnNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Die Daten werden von einer anderen Anfrage bearbeitet. Bitte warten Sie, bis die vorherige Anfrage beendet ist und wiederholen Sie die Aktion..
        /// </summary>
        internal static string ErrorCommandInProgress {
            get {
                return ResourceManager.GetString("ErrorCommandInProgress", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fehlende Berechtigung zum Anlegen neuer Datensätze..
        /// </summary>
        internal static string ErrorCreateRecordNotAllowed {
            get {
                return ResourceManager.GetString("ErrorCreateRecordNotAllowed", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Dashboard Ansicht nicht gefunden..
        /// </summary>
        internal static string ErrorDashboardViewNotFound {
            get {
                return ResourceManager.GetString("ErrorDashboardViewNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Kritischer Fehler: Daten wurden während dem Ändern der aktuellen Zeile nicht gespeichert. Daten würden dadurch inkonsistent. Bitte Seite neu laden und nochmals versuchen..
        /// </summary>
        internal static string ErrorDataNotSavedWhileChangingRow {
            get {
                return ResourceManager.GetString("ErrorDataNotSavedWhileChangingRow", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zeile `{0}&apos; abhängig von Zeile `{1}&apos; bei Entität `{2}&apos;, ({3}) konnte nicht aktualisiert werden..
        /// </summary>
        internal static string ErrorDependentColumnNotFound {
            get {
                return ResourceManager.GetString("ErrorDependentColumnNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Export nicht mehr verfügbar..
        /// </summary>
        internal static string ErrorExportNotAvailable {
            get {
                return ResourceManager.GetString("ErrorExportNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sie müssen die Formulareinträge speichern, bevor Sie die aktuelle Aktion ausführen..
        /// </summary>
        internal static string ErrorFormNotSavedBeforeAction {
            get {
                return ResourceManager.GetString("ErrorFormNotSavedBeforeAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Formulareintrag enthält Fehler (einige Plichtfelder sind leer). Beheben Sie diese Fehler, bevor Sie weiterfahren..
        /// </summary>
        public static string ErrorInForm {
            get {
                return ResourceManager.GetString("ErrorInForm", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Liste der Auftäge in der Warteschlange konnte nicht geladen werden..
        /// </summary>
        internal static string ErrorLoadingWorkQueueList {
            get {
                return ResourceManager.GetString("ErrorLoadingWorkQueueList", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sitzung nicht verfügbar. Ausloggen nicht möglich..
        /// </summary>
        internal static string ErrorLogOut {
            get {
                return ResourceManager.GetString("ErrorLogOut", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Modus wird nicht unterstützt. Parameter für das Auswählen von mehreren Datensätzen können nicht weitergereicht werden..
        /// </summary>
        internal static string ErrorMultipleRecordsParameters {
            get {
                return ResourceManager.GetString("ErrorMultipleRecordsParameters", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Keine Datensätze ausgewählt. Aktion kann nicht ausgeführt werden..
        /// </summary>
        internal static string ErrorNoRecordsSelectedForAction {
            get {
                return ResourceManager.GetString("ErrorNoRecordsSelectedForAction", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auswahl mehrerer Datensätze für OpenForm wir nicht unterstützt..
        /// </summary>
        internal static string ErrorOpenFormMultipleRecords {
            get {
                return ResourceManager.GetString("ErrorOpenFormMultipleRecords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Pseudoparameter &apos;body&apos; ist nicht als Paramter einer API Datenseite definiert, wurde aber in einem PUT Call verwendet..
        /// </summary>
        internal static string ErrorPseudoparameterBodyNotDefined {
            get {
                return ResourceManager.GetString("ErrorPseudoparameterBodyNotDefined", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Tabellenzeile wird von der mitgelieferten ID nicht gefunden..
        /// </summary>
        internal static string ErrorRecordNotFound {
            get {
                return ResourceManager.GetString("ErrorRecordNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ungültige Datenstruktur von {0} zurückgegeben für Aktion {1}..
        /// </summary>
        internal static string ErrorRefreshReturnInvalid {
            get {
                return ResourceManager.GetString("ErrorRefreshReturnInvalid", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auswahl mehrerer Datensätze für ReportAction wird nicht unterstützt..
        /// </summary>
        internal static string ErrorReportActionMultipleRecords {
            get {
                return ResourceManager.GetString("ErrorReportActionMultipleRecords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Protokoll nicht mehr verfügbar..
        /// </summary>
        internal static string ErrorReportNotAvailable {
            get {
                return ResourceManager.GetString("ErrorReportNotAvailable", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Auswahl mehrerer Datensätze für OpenForm/Report wird nicht unterstützt..
        /// </summary>
        internal static string ErrorReportOpenFormMultipleRecords {
            get {
                return ResourceManager.GetString("ErrorReportOpenFormMultipleRecords", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ihre Verbindung ist abgelaufen oder die Anwendung wurde neu gestartet. Bitte aktualisieren Sie Ihren Browser oder melden Sie sich erneut an..
        /// </summary>
        internal static string ErrorSessionExpired {
            get {
                return ResourceManager.GetString("ErrorSessionExpired", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sitzung nicht gefunden: {0}.
        /// </summary>
        internal static string ErrorSessionNotFound {
            get {
                return ResourceManager.GetString("ErrorSessionNotFound", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Zu viele Fenster geöffnet. Bitte schliessen Sie unbenötigte Reiter vor dem Öffnen neuer Fenster..
        /// </summary>
        internal static string ErrorTooManyTabsOpen {
            get {
                return ResourceManager.GetString("ErrorTooManyTabsOpen", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der Knoten kann nicht gelöscht werden. Bitte vorgängig alle Unterknoten löschen..
        /// </summary>
        internal static string ErrorTreeHasChildrenCannotDelete {
            get {
                return ResourceManager.GetString("ErrorTreeHasChildrenCannotDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Der Knoten kann nicht gelöscht werden, da er von anderen Daten verwendet wird..
        /// </summary>
        internal static string ErrorTreeItemUsedCannotDelete {
            get {
                return ResourceManager.GetString("ErrorTreeItemUsedCannotDelete", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Workflow für die UIWorkflowAction nicht definiert..
        /// </summary>
        internal static string ErrorWorkflowNotSet {
            get {
                return ResourceManager.GetString("ErrorWorkflowNotSet", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Anzahl der exportierten Dateien hat die vom Administrator gesetzte Limite überschritten..
        /// </summary>
        internal static string ExportLimitExceeded {
            get {
                return ResourceManager.GetString("ExportLimitExceeded", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Sie haben keinen Zugriff auf {0}.
        /// </summary>
        internal static string MenuNotAuthorized {
            get {
                return ResourceManager.GetString("MenuNotAuthorized", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Fehler im Arbeitsablauf.
        /// </summary>
        internal static string WorkflowErrorTitle {
            get {
                return ResourceManager.GetString("WorkflowErrorTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arbeitsfluss wurde erfolgreich beendet.
        /// </summary>
        internal static string WorkflowFinished {
            get {
                return ResourceManager.GetString("WorkflowFinished", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Ende des Arbeitsflusses.
        /// </summary>
        internal static string WorkflowFinishedTitle {
            get {
                return ResourceManager.GetString("WorkflowFinishedTitle", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Arbeitsvorrat.
        /// </summary>
        internal static string WorkQueueTitle {
            get {
                return ResourceManager.GetString("WorkQueueTitle", resourceCulture);
            }
        }
    }
}
