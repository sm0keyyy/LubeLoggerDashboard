using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace LubeLoggerDashboard.Tests.TestHelpers.UI
{
    /// <summary>
    /// Helper class for WPF UI testing
    /// </summary>
    public static class WpfTestHelper
    {
        /// <summary>
        /// Executes the specified action on the UI thread and waits for completion
        /// </summary>
        /// <param name="action">The action to execute</param>
        public static void RunOnUIThread(Action action)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("No WPF application is running. Make sure to initialize the application before using this method.");
            }

            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                action();
            }
            else
            {
                dispatcher.Invoke(action);
            }
        }

        /// <summary>
        /// Executes the specified function on the UI thread and waits for the result
        /// </summary>
        /// <typeparam name="T">The return type of the function</typeparam>
        /// <param name="func">The function to execute</param>
        /// <returns>The result of the function</returns>
        public static T RunOnUIThread<T>(Func<T> func)
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("No WPF application is running. Make sure to initialize the application before using this method.");
            }

            var dispatcher = Application.Current.Dispatcher;
            if (dispatcher.CheckAccess())
            {
                return func();
            }
            else
            {
                return dispatcher.Invoke(func);
            }
        }

        /// <summary>
        /// Waits for the UI thread to process all pending operations
        /// </summary>
        public static void WaitForUIThread()
        {
            if (Application.Current == null)
            {
                throw new InvalidOperationException("No WPF application is running. Make sure to initialize the application before using this method.");
            }

            RunOnUIThread(() => 
            {
                var frame = new DispatcherFrame();
                Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background,
                    new DispatcherOperationCallback(f => 
                    {
                        ((DispatcherFrame)f).Continue = false;
                        return null;
                    }), frame);
                Dispatcher.PushFrame(frame);
            });
        }

        /// <summary>
        /// Waits for a condition to be true with a timeout
        /// </summary>
        /// <param name="condition">The condition to wait for</param>
        /// <param name="timeout">The maximum time to wait</param>
        /// <param name="interval">The interval between condition checks</param>
        /// <returns>True if the condition was met within the timeout, false otherwise</returns>
        public static bool WaitForCondition(Func<bool> condition, TimeSpan timeout, TimeSpan? interval = null)
        {
            var checkInterval = interval ?? TimeSpan.FromMilliseconds(100);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            while (stopwatch.Elapsed < timeout)
            {
                if (RunOnUIThread(condition))
                {
                    return true;
                }
                
                Thread.Sleep(checkInterval);
            }
            
            return false;
        }

        /// <summary>
        /// Waits for a condition to be true with a timeout (async version)
        /// </summary>
        /// <param name="condition">The condition to wait for</param>
        /// <param name="timeout">The maximum time to wait</param>
        /// <param name="interval">The interval between condition checks</param>
        /// <returns>True if the condition was met within the timeout, false otherwise</returns>
        public static async Task<bool> WaitForConditionAsync(Func<bool> condition, TimeSpan timeout, TimeSpan? interval = null)
        {
            var checkInterval = interval ?? TimeSpan.FromMilliseconds(100);
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            
            while (stopwatch.Elapsed < timeout)
            {
                if (RunOnUIThread(condition))
                {
                    return true;
                }
                
                await Task.Delay(checkInterval);
            }
            
            return false;
        }

        /// <summary>
        /// Clicks a button in the UI
        /// </summary>
        /// <param name="button">The button to click</param>
        public static void ClickButton(Button button)
        {
            if (button == null)
            {
                throw new ArgumentNullException(nameof(button));
            }
            
            RunOnUIThread(() => 
            {
                if (!button.IsEnabled)
                {
                    throw new InvalidOperationException($"Button '{button.Name}' is disabled and cannot be clicked.");
                }
                
                button.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
            });
        }

        /// <summary>
        /// Sets the text of a TextBox
        /// </summary>
        /// <param name="textBox">The TextBox to set</param>
        /// <param name="text">The text to set</param>
        public static void SetTextBoxText(TextBox textBox, string text)
        {
            if (textBox == null)
            {
                throw new ArgumentNullException(nameof(textBox));
            }
            
            RunOnUIThread(() => 
            {
                if (!textBox.IsEnabled)
                {
                    throw new InvalidOperationException($"TextBox '{textBox.Name}' is disabled and cannot be edited.");
                }
                
                textBox.Text = text;
                textBox.GetBindingExpression(TextBox.TextProperty)?.UpdateSource();
            });
        }

        /// <summary>
        /// Selects an item in a ComboBox
        /// </summary>
        /// <param name="comboBox">The ComboBox to set</param>
        /// <param name="itemIndex">The index of the item to select</param>
        public static void SelectComboBoxItem(ComboBox comboBox, int itemIndex)
        {
            if (comboBox == null)
            {
                throw new ArgumentNullException(nameof(comboBox));
            }
            
            RunOnUIThread(() => 
            {
                if (!comboBox.IsEnabled)
                {
                    throw new InvalidOperationException($"ComboBox '{comboBox.Name}' is disabled and cannot be edited.");
                }
                
                if (itemIndex < 0 || itemIndex >= comboBox.Items.Count)
                {
                    throw new ArgumentOutOfRangeException(nameof(itemIndex), $"Index {itemIndex} is out of range for ComboBox with {comboBox.Items.Count} items.");
                }
                
                comboBox.SelectedIndex = itemIndex;
            });
        }

        /// <summary>
        /// Selects an item in a ComboBox by its value
        /// </summary>
        /// <param name="comboBox">The ComboBox to set</param>
        /// <param name="item">The item to select</param>
        public static void SelectComboBoxItem(ComboBox comboBox, object item)
        {
            if (comboBox == null)
            {
                throw new ArgumentNullException(nameof(comboBox));
            }
            
            RunOnUIThread(() => 
            {
                if (!comboBox.IsEnabled)
                {
                    throw new InvalidOperationException($"ComboBox '{comboBox.Name}' is disabled and cannot be edited.");
                }
                
                comboBox.SelectedItem = item;
            });
        }

        /// <summary>
        /// Checks or unchecks a CheckBox
        /// </summary>
        /// <param name="checkBox">The CheckBox to set</param>
        /// <param name="isChecked">Whether the CheckBox should be checked</param>
        public static void SetCheckBoxState(CheckBox checkBox, bool isChecked)
        {
            if (checkBox == null)
            {
                throw new ArgumentNullException(nameof(checkBox));
            }
            
            RunOnUIThread(() => 
            {
                if (!checkBox.IsEnabled)
                {
                    throw new InvalidOperationException($"CheckBox '{checkBox.Name}' is disabled and cannot be edited.");
                }
                
                checkBox.IsChecked = isChecked;
            });
        }
    }
}