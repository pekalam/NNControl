using System.ComponentModel;
using System.Windows;
using NeuralNetworkControl.Abstraction;
using NeuralNetworkControl.Impl;
using StateMachineLib;

namespace NeuralNetworkControl
{
    public partial class NeuralNetworkControl
    {
        enum Triggers
        {
            LBx2, NEURON_HIT, SYNAPSE_HIT, CTRL_LB, LBx1, MV_LB, RBx1, BACKGROUND_HIT, LB_UP, ALL_NEURONS_DESELECT, SYNAPSE_DESELECT,
            ZOOM
        }

        enum States
        {
            S0, S1, S2, S3, S4, S5, S6, S7, S8, S9, S10, S11, S12, S13, ZOOM_INTERRUPT
        }

        private StateMachine<Triggers, States> _stateMachine;
        private StateMachineVis<Triggers, States> vis;


        private Point _clickedPoint;
        private Point _baseMovePoint;
        private SynapseViewAbstraction _clickedSynapse;
        private NeuronViewAbstraction _clickedNeuron;
        private Point _clickedNeuronScreenPoint;


        private void InitStateMachine()
        {
            _stateMachine = new StateMachineBuilder<Triggers, States>()
                .CreateState(States.S0)
                    .Transition(Triggers.LBx1, States.S12)
                    .Transition(Triggers.LBx2, States.S1)
                    .Transition(Triggers.CTRL_LB, States.S1)
                    .Loop(Triggers.RBx1)
                    .Loop(Triggers.LB_UP)
                    .Loop(Triggers.MV_LB)
                .End()

                .CreateState(States.S1)
                    .Enter(s =>
                    {
                        if ((_clickedNeuron = _networkView.SelectNeuronAt((int)_clickedPoint.X, (int)_clickedPoint.Y)) != null)
                        {
                            _clickedNeuronScreenPoint = _clickedPoint;
                            _stateMachine.Next(Triggers.NEURON_HIT);
                        }
                        else if ((_clickedSynapse = _networkView.SelectSynapseAt((int)_clickedPoint.X, (int)_clickedPoint.Y)) != null)
                        {
                            _stateMachine.Next(Triggers.SYNAPSE_HIT);
                        }
                        else
                        {
                            _stateMachine.Next(Triggers.BACKGROUND_HIT);
                        }
                    })
                    .Loop(Triggers.LBx2)
                    .Transition(Triggers.NEURON_HIT, States.S2)
                    .Transition(Triggers.SYNAPSE_HIT, States.S3)
                    .Transition(Triggers.BACKGROUND_HIT, States.S0)
                .End()

                .CreateState(States.S2)
                    .Enter(s =>
                    {
                        if (_clickedSynapse != null)
                        {
                            _clickedSynapse = null;
                            _networkView.DeselectSynapse();
                        }

                        if (_networkView.Impl.SelectedNeuron.Count == 1)
                        {
                            _clickedNeuronScreenPoint.X = _networkView.Impl.SelectedNeuron[0].X;
                            _clickedNeuronScreenPoint.Y = _networkView.Impl.SelectedNeuron[0].Y;
                            ShowNeuronOverlay(_clickedNeuronScreenPoint);
                            RaiseNeuronClickEvent(_networkView.Impl.SelectedNeuron[0]);
                        }
                        else
                        {
                            HideNeuronOverlay();
                        }
                    })
                    .Transition(Triggers.LBx1, States.S6)
                    .Transition(Triggers.CTRL_LB, States.S5)
                    .Transition(Triggers.RBx1, States.S8)
                    .Loop(Triggers.LB_UP)
                    .Loop(Triggers.MV_LB)
                    .Loop(Triggers.LBx2)
                .End()

                .CreateState(States.S3)
                .Enter(s =>
                {

                })
                .Loop(Triggers.LB_UP)
                .Loop(Triggers.LBx1)
                .Loop(Triggers.MV_LB)
                .Loop(Triggers.LBx2)
                .Transition(Triggers.RBx1, States.S4)
                .End()

                .CreateState(States.S4)
                .Enter(s =>
                {
                    _clickedSynapse = null;
                    _networkView.DeselectSynapse();
                    _stateMachine.Next(Triggers.SYNAPSE_DESELECT);
                })
                .Transition(Triggers.SYNAPSE_DESELECT, States.S0)
                .End()

                .CreateState(States.S5)
                    .Enter(s =>
                    {
                        if ((_clickedNeuron = _networkView.SelectNeuronAt((int)_clickedPoint.X, (int)_clickedPoint.Y)) != null)
                        {
                            _stateMachine.Next(Triggers.NEURON_HIT);
                        }
                        else
                        {
                            _stateMachine.Next(Triggers.BACKGROUND_HIT);
                        }
                    })
                    .Transition(Triggers.NEURON_HIT, States.S2)
                    .Transition(Triggers.BACKGROUND_HIT, States.S2)
                .End()

                .CreateState(States.S6)
                    .Enter(s =>
                    {
                        _clickedNeuron = _networkView.FindNeuronAt((int)_clickedPoint.X, (int)_clickedPoint.Y);
                        if (_clickedNeuron == null)
                        {
                            _stateMachine.Next(Triggers.BACKGROUND_HIT);
                        }
                        else if (_networkView.Impl.SelectedNeuron.Contains(_clickedNeuron.Impl))
                        {
                            _stateMachine.Next(Triggers.NEURON_HIT);
                        }
                        else
                        {
                            _stateMachine.Next(Triggers.BACKGROUND_HIT);
                        }
                    })
                    .Transition(Triggers.BACKGROUND_HIT, States.S9)
                    .Transition(Triggers.NEURON_HIT, States.S10)
                    .Transition(Triggers.MV_LB, States.S2)
                    .Transition(Triggers.LB_UP, States.S2)
                .End()


                .CreateState(States.S7)
                    .Enter(s =>
                    {
                        _networkView.MoveSelectedNeurons(_clickedNeuron.Impl, (float)_clickedPoint.X, (float)_clickedPoint.Y);
                        _clickedNeuronScreenPoint = _clickedPoint;
                        if (_networkView.Impl.SelectedNeuron.Count == 1)
                        {
                            _clickedNeuronScreenPoint.X = _networkView.Impl.SelectedNeuron[0].X;
                            _clickedNeuronScreenPoint.Y = _networkView.Impl.SelectedNeuron[0].Y;
                            ShowNeuronOverlay(_clickedNeuronScreenPoint);
                            RaiseNeuronClickEvent(_networkView.Impl.SelectedNeuron[0]);
                        }
                        else
                        {
                            HideNeuronOverlay();
                        }
                    })
                    .Exit(s =>
                    {
                        if (s == Triggers.LB_UP)
                        {
                            _networkView.SelectedNeuronMoveEnd();
                        }
                    })
                    .Transition(Triggers.LB_UP, States.S2)
                    .Loop(Triggers.MV_LB)
                    .Ignoring()
                .End()



                .CreateState(States.S8)
                    .Enter(s =>
                    {
                        _clickedNeuron = null;
                        _networkView.DeselectNeuron();
                        HideNeuronOverlay();
                        _stateMachine.Next(Triggers.ALL_NEURONS_DESELECT);
                    })
                    .Transition(Triggers.ALL_NEURONS_DESELECT, States.S0)
                .End()


                .CreateState(States.S9)
                .Enter(s =>
                {
                    _baseMovePoint = _clickedPoint;
                    HideNeuronOverlay();
                })
                .Transition(Triggers.MV_LB, States.S11)
                .Transition(Triggers.LB_UP, States.S2)
                .End()


                .CreateState(States.S10)
                    .Transition(Triggers.MV_LB, States.S7)
                    .Transition(Triggers.LB_UP, States.S2)
                .End()


                .CreateState(States.S11)
                    .Enter(s =>
                    {
                        _networkView.Move((int)(_clickedPoint.X - _baseMovePoint.X), (int)(_clickedPoint.Y - _baseMovePoint.Y));
                        _baseMovePoint = _clickedPoint;
                    })
                .Loop(Triggers.MV_LB)
                    .Ignoring()
                    .Transition(Triggers.LB_UP, States.S2)
                .End()



                .CreateState(States.S12)
                    .Enter(s =>
                    {
                        _baseMovePoint = _clickedPoint;
                    })
                    .Transition(Triggers.MV_LB, States.S13)
                    .Transition(Triggers.LB_UP, States.S0)
                .End()


                .CreateState(States.S13)
                .Enter(s =>
                {
                    _networkView.Move((int)(_clickedPoint.X - _baseMovePoint.X), (int)(_clickedPoint.Y - _baseMovePoint.Y));
                    _baseMovePoint = _clickedPoint;
                })
                .Transition(Triggers.LB_UP, States.S0)
                .Loop(Triggers.MV_LB)
                .Ignoring()
                .End()
                
                .ResetInterruptState(Triggers.ZOOM, t =>
                {
                    HideNeuronOverlay();
                }, States.ZOOM_INTERRUPT, States.S0)

                .Build(States.S0);


            if (!DesignerProperties.GetIsInDesignMode(this) && (bool)GetValue(ShowVisProperty))
            {
                vis = new StateMachineVis<Triggers, States>(_stateMachine);
                vis.Start(@"StateMachineLibVis.exe", "-c graphViz -l 0");
                _networkView.StartVis();
            }


        }

    }
}