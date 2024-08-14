using System.Linq;
using FullyExternalCS2.Data.Entity;
using FullyExternalCS2.Utils;

namespace FullyExternalCS2.Data.Game;

public class GameData : ThreadedServiceBase
{
    #region properties

    protected override string ThreadName => nameof(GameData);

    private GameProcess GameProcess { get; set; }

    public Player Player { get; private set; }

    public Entity.Entity[] Entities { get; private set; }

    #endregion

    #region methods

    /// <inheritdoc />
    public GameData(GameProcess gameProcess)
    {
        GameProcess = gameProcess;
        Player = new Player();
        Entities = Enumerable.Range(0, 64).Select(index => new Entity.Entity(index)).ToArray();
    }

    public override void Dispose()
    {
        base.Dispose();

        Entities = default;
        Player = default;
        GameProcess = default;
    }

    protected override void FrameAction()
    {
        if (!GameProcess.IsValid) return;
        Player.Update(GameProcess);

        foreach (var entity in Entities) entity.Update(GameProcess);
    }

    #endregion
}