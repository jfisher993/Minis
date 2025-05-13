using System.Collections.Generic;
using RtMidiIn = RtMidi.MidiIn;

namespace Minis {

//
// MIDI device driver class that manages detected MIDI ports
//
sealed class MidiDriver : System.IDisposable
{
    #region Private objects and methods

    RtMidiIn _probe;
    List<MidiPort> _ports = new List<MidiPort>();

    void OpenAllAvailablePorts()
    {
        for (var i = 0; i < _probe.PortCount; i++)
            _ports.Add(new MidiPort(i, _probe.GetPortName(i)));
    }

    void CloseAllPorts()
    {
        foreach (var p in _ports) p.Dispose();
        _ports.Clear();
    }

    #endregion

    #region Public methods

    public void Update()
    {
        if (_probe == null) _probe = RtMidiIn.Create();

        // Port rescan triggered by port count mismatch
        if (_ports.Count != _probe.PortCount)
        {
            CloseAllPorts();
            OpenAllAvailablePorts();
        }

        // Queued MIDI event processing
        foreach (var p in _ports) p.ProcessEventQueue();
    }

    public void Dispose()
    {
        CloseAllPorts();
        _probe?.Dispose();
        _probe = null;
    }

    #endregion
}

} // namespace Minis
