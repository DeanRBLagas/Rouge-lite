using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class ReadyManager : MonoBehaviourPunCallbacks
{
    public void OnReadyButtonClicked()
    {
        Hashtable props = new Hashtable
        {
            { "IsReady", true }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(props);
    }

    private void Update()
    {
        if (AllPlayersReady())
        {
            PhotonNetwork.LoadLevel("SampleScene");
        }
    }

    private bool AllPlayersReady()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.ContainsKey("IsReady") || !(bool)player.CustomProperties["IsReady"])
            {
                return false;
            }
        }
        return true;
    }
}
