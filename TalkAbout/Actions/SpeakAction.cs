using Microsoft.Xaml.Interactivity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TalkAbout.Model;
using TalkAbout.ViewModel;
using Windows.Media.SpeechSynthesis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace TalkAbout.Actions
{
    public class SpeakAction : DependencyObject, IAction, ITaskCompleteNotifier
    {
        public static readonly DependencyProperty TargetObjectProperty =
            DependencyProperty.Register(
                "TargetObject",
                typeof(object),
                typeof(SpeakAction),
                null);

        public static readonly DependencyProperty UtteranceProperty =
            DependencyProperty.Register(
                "Utterance",
                typeof(string),
                typeof(SpeakAction),
                null);

        public object TargetObject
        {
            get
            {
                return GetValue(TargetObjectProperty);
            }
            set
            {
                SetValue(TargetObjectProperty, value);
            }
        }

        public string Utterance
        {
            get
            {
                return (string)(GetValue(UtteranceProperty));
            }
            set
            {
                SetValue(UtteranceProperty, value);
            }
        }

        private SpeechSynthesizer _synthesiser;
        private MediaElement _media;
        private bool _speaking;
        private TaskNotifier _notifier;

        public SpeakAction()
        {
            _speaking = false;
            _notifier = new TaskNotifier(this);
            _synthesiser = new SpeechSynthesizer();
            
        }

        public object Execute(object sender, object parameter)
        {
            bool result = false;
            object target = null;
            if (_media == null && ReadLocalValue(TargetObjectProperty) != DependencyProperty.UnsetValue)
            {
                target = GetValue(TargetObjectProperty);
                if (target != null)
                {
                    if (target is MediaElement)
                    {
                        _media = (MediaElement)target;
                    }
                }
            }
            
            if (!string.IsNullOrWhiteSpace(Utterance) && !_speaking && _media != null)
            {
                
                _synthesiser.Voice = Settings.Instance.SettingsVoice;
                _speaking = true;
                _notifier.Execute(_synthesise(), "speak");
            }
            
            return result;
        }

        private async Task _synthesise()
        {
            try
            {
                
                SpeechSynthesisStream stream = await _synthesiser.SynthesizeTextToStreamAsync(Utterance);
                _media.AutoPlay = true;
                _media.SetSource(stream, stream.ContentType);
                _media.Volume = (double)(Settings.Instance.VoiceVolume / 100);
                _media.Play(); 
                
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void TaskComplete(Task task, string identifier)
        {
            switch (identifier)
            {
                case "speak":
                    {
                        _speaking = false;
                        break;
                    }
                default:
                    {
                        break;
                    }
                    
            }
        }
    }
}
