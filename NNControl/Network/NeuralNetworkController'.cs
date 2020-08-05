using StateMachineLib;
using System;
using System.Collections.Generic;

namespace NNControl.Network
{
    public partial class NeuralNetworkController : IDisposable
    {
        public enum ViewTrig
        {
            DRAW, PAINT, SELECT, ZOOM, DESELECT, MV, MV_END, FORCE_DRAW, SELECT_SYNAPSE, DESELECT_SYNAPSE, FORCE_DRAW_EXCLUDED,
            FORCE_DRAW_FROM_SAVED, REPOSITION
        }

        public enum States
        {
            S0, S1, S2, S3, S4, S5, S6, S7, S8, ZOOM_INTERRUPT, FORCE_DRAW_INTERRUPT, S9, S10, S11, FORCE_DRAW_EXCLUDED_INT,
            FORCE_DRAW_FROM_SAVED_INT, REPOSITION_INT
        }

        public enum RedrawStates
        {
            S0, S1, S2
        }

        private readonly StateMachine<ViewTrig, States> _stateMachine;
        private readonly StateMachine<ViewTrig, RedrawStates> _redrawStateMachine;
        private StateMachineVis<ViewTrig, States> vis;

        private readonly Queue<ViewTrig> _redrawMachineQueue = new Queue<ViewTrig>();

        protected NeuralNetworkController(Action onRedraw)
        {
            _onRedraw = onRedraw;
            _redrawStateMachine = new StateMachineBuilder<ViewTrig, RedrawStates>()
                .CreateState(RedrawStates.S0)
                    .Enter(s =>
                    {
                        _redrawMachineQueue.Enqueue(ViewTrig.PAINT);
                    })
                    .Transition(ViewTrig.DRAW, RedrawStates.S1)
                    .AllTransition(RedrawStates.S2)
                .End()


                .CreateState(RedrawStates.S1)
                    .Enter(s =>
                    {
                        if (_redrawMachineQueue.Count == 0)
                        {
                            _stateMachine.Next(ViewTrig.PAINT);
                        }
                        else
                        {
                            while (_redrawMachineQueue.Count > 0)
                            {
                                _redrawMachineQueue.TryDequeue(out var next);
                                _stateMachine.Next(next);
                            }
                        }
                    })
                    .Transition(ViewTrig.DRAW, RedrawStates.S1)
                    .AllTransition(RedrawStates.S2)
                .End()


                .CreateState(RedrawStates.S2)
                    .Enter(s =>
                    {
                        _redrawMachineQueue.Enqueue(s.Trigger);
                    })
                    .Transition(ViewTrig.DRAW, RedrawStates.S1)
                    .AllTransition(RedrawStates.S2)
                .End()
                .Build(RedrawStates.S0);







            _stateMachine = new StateMachineBuilder<ViewTrig, States>()

                .CreateState(States.S0)
                    .Transition(ViewTrig.PAINT, States.S1)
                .End()


                .CreateState(States.S1)
                    .Enter(s => { View.DrawAndSave(); })
                    .Transition(ViewTrig.PAINT, States.S2)
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.SELECT_SYNAPSE, States.S9)
                .End()


                .CreateState(States.S2)
                    .Enter(s => { View.DrawFromSaved(); })
                    .Transition(ViewTrig.PAINT, States.S2)
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.SELECT_SYNAPSE, States.S9)
                .End()


                .CreateState(States.S3)
                    .Enter(s =>
                    {
                        View.DrawAndSave();
                    })
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.DESELECT, States.S5)
                    .Transition(ViewTrig.PAINT, States.S4)
                    .Transition(ViewTrig.MV, States.S6)
                .End()


                .CreateState(States.S4)
                    .Enter(s => { View.DrawFromSaved(); })
                    .Transition(ViewTrig.PAINT, States.S4)
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.DESELECT, States.S5)
                    .Transition(ViewTrig.MV, States.S6)
                .End()


                .CreateState(States.S5)
                    .Enter(s =>
                    {
                        SetSelectedNeuronsAndSynapsesExcluded(false);

                        View.DrawAndSave();
                    })
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.SELECT_SYNAPSE, States.S9)
                    .Transition(ViewTrig.PAINT, States.S2)
                    .Transition(ViewTrig.DESELECT, States.S5)

                .End()


                .CreateState(States.S6)
                    .Enter(s =>
                    {
                        SetSelectedNeuronsAndSynapsesExcluded(true);

                        View.DrawAndSave();
                        View.DrawExcluded();
                    })
                    .Transition(ViewTrig.DESELECT, States.S5)
                    .Transition(ViewTrig.MV, States.S8)
                    .Transition(ViewTrig.PAINT, States.S8)
                    .Transition(ViewTrig.MV_END, States.S7)
                .End()


                .CreateState(States.S7)
                    .Enter(s =>
                    {
                        SetSelectedNeuronsAndSynapsesExcluded(false);
                        View.DrawAndSave();
                    })
                    .Transition(ViewTrig.PAINT, States.S4)
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.DESELECT, States.S5)
                    .Transition(ViewTrig.MV, States.S6)
                .End()


                .CreateState(States.S8)
                    .Enter(s => {
                        View.DrawFromSaved();
                        View.DrawExcluded();
                    })
                    .Transition(ViewTrig.PAINT, States.S8)
                    .Transition(ViewTrig.MV, States.S8)
                    .Transition(ViewTrig.DESELECT, States.S5)
                    .Transition(ViewTrig.MV_END, States.S7)
                    .Transition(ViewTrig.SELECT, States.S3)
                .End()



                .CreateState(States.S9)
                .Enter(s =>
                {
                    View.SelectedSynapse.Excluded = true;
                    View.SelectedSynapse.Neuron1.Excluded = true;
                    View.SelectedSynapse.Neuron2.Excluded = true;
                    View.DrawAndSave();
                    View.DrawExcluded();
                })
                .Transition(ViewTrig.ZOOM, States.S11)
                .Transition(ViewTrig.REPOSITION, States.S11)
                .Transition(ViewTrig.PAINT, States.S10)
                .Transition(ViewTrig.DESELECT_SYNAPSE, States.S11)
                .End()


                .CreateState(States.S10)
                .Enter(s =>
                {
                    View.DrawFromSaved();
                    View.DrawExcluded();
                })
                .Loop(ViewTrig.PAINT)
                .Transition(ViewTrig.DESELECT_SYNAPSE, States.S11)
                .End()


                .CreateState(States.S11)
                    .Enter(s =>
                    {
                        SmDeselectSynapses();
                        View.DrawAndSave();
                    })
                    .Transition(ViewTrig.SELECT_SYNAPSE, States.S9)
                    .Transition(ViewTrig.SELECT, States.S3)
                    .Transition(ViewTrig.PAINT, States.S2)

                .End()


                .InterruptState(ViewTrig.ZOOM, s =>
                {
                    SmDeselectNeurons();
                    View.DrawAndSave();

                }, States.ZOOM_INTERRUPT)

                .InterruptState(ViewTrig.FORCE_DRAW, s =>
                {
                    View.DrawAndSave();
                    View.DrawExcluded();
                }, States.FORCE_DRAW_INTERRUPT)


                .InterruptState(ViewTrig.REPOSITION, s =>
                {
                    SmDeselectNeurons();
                    SmDeselectSynapses();
                    View.DrawAndSave();
                }, States.REPOSITION_INT)

                .InterruptState(ViewTrig.FORCE_DRAW_EXCLUDED, s =>
                {
                    View.DrawFromSaved();
                    View.DrawExcluded();
                }, States.FORCE_DRAW_EXCLUDED_INT)

                .InterruptState(ViewTrig.FORCE_DRAW_FROM_SAVED, s =>
                {
                    View.DrawAndSave();
                }, States.FORCE_DRAW_FROM_SAVED_INT)

                .Build(States.S0);

        }


        internal void StartVis()
        {
            vis = new StateMachineVis<ViewTrig, States>(_stateMachine, "viewGraphViz");
            vis.Start(@"StateMachineLibVis.exe", "-c viewGraphViz -l 970");
        }

        private void SetSelectedNeuronsAndSynapsesExcluded(bool excluded)
        {
            foreach (var neuron in View.SelectedNeuron)
            {
                neuron.Excluded = excluded;
                foreach (var synapse in neuron.ConnectedSynapses)
                {
                    synapse.Excluded = excluded;
                    // synapse.Neuron1.Excluded = excluded;
                    // synapse.Neuron2.Excluded = excluded;
                }

                foreach (var synapse in neuron.Synapses)
                {
                    synapse.Excluded = excluded;
                }
            }
        }

        private void SmDeselectNeurons()
        {
            if (View.SelectedNeuron.Count > 0)
            {
                SetSelectedNeuronsAndSynapsesExcluded(false);
                _selectedNeurons.Clear();
                View.SelectedNeuron.Clear();
            }
        }

        private void SmDeselectSynapses()
        {
            if (View.SelectedSynapse != null)
            {
                View.SelectedSynapse.Excluded = false;
                View.SelectedSynapse.Neuron1.Excluded = false;
                View.SelectedSynapse.Neuron2.Excluded = false;
                View.SelectedSynapse = null;
            }


        }

        public void Dispose()
        {
            vis?.Dispose();
        }
    }
}