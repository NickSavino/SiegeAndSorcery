using System.Collections.Generic;
namespace _Scripts.Buildings
{
    public sealed class BuildableStructureSnapSolver
    {
        public bool TryFindSnapCandidate(
            BuildableStructure preview,
            IReadOnlyList<BuildableStructure> placedStructures,
            float snapDistance,
            out ConnectionCandidate previewCandidate,
            out ConnectionCandidate targetCandidate)
        {
            ConnectionCandidate bestPreview = default;
            ConnectionCandidate bestTarget = default;

            bool found = false;
            float closestDistanceSquared = snapDistance * snapDistance;

            foreach (var building in placedStructures)
            {
                if (building == null ||
                    building == preview ||
                    !building.isActiveAndEnabled ||
                    !building.IsPlaced())
                {
                    continue;
                }

                foreach (var source in preview.Sockets)
                {
                    if (!IsAvailable(source))
                    {
                        continue;
                    }

                    var sourceCandidate = new ConnectionCandidate(source);

                    foreach (var target in building.Sockets)
                    {
                        if (IsAvailable(target) &&
                            source.CanConnectTo(target))
                        {
                            Consider(
                                sourceCandidate,
                                new ConnectionCandidate(target)
                            );
                        }
                    }

                    var targetSurface = building.ConnectionSurface;

                    if (targetSurface != null &&
                        targetSurface.isActiveAndEnabled &&
                        targetSurface.TryGetCandidate(
                            source.transform.position,
                            out var surfaceCandidate))
                    {
                        Consider(sourceCandidate, surfaceCandidate);
                    }
                }

                var previewSurface = preview.ConnectionSurface;

                if (previewSurface == null ||
                    !previewSurface.isActiveAndEnabled)
                {
                    continue;
                }

                foreach (var target in building.Sockets)
                {
                    if (!IsAvailable(target))
                    {
                        continue;
                    }

                    if (previewSurface.TryGetCandidate(
                        target.transform.position,
                        out var surfaceCandidate))
                    {
                        Consider(
                            surfaceCandidate,
                            new ConnectionCandidate(target)
                        );
                    }
                }
            }

            previewCandidate = bestPreview;
            targetCandidate = bestTarget;
            return found;

            void Consider(
                ConnectionCandidate source,
                ConnectionCandidate target)
            {
                if (source.Owner == null ||
                    target.Owner == null ||
                    source.Owner == target.Owner)
                {
                    return;
                }

                float distanceSquared =
                    (source.Position - target.Position).sqrMagnitude;

                if (distanceSquared >= closestDistanceSquared)
                {
                    return;
                }

                closestDistanceSquared = distanceSquared;
                bestPreview = source;
                bestTarget = target;
                found = true;
            }
        }

        static bool IsAvailable(BuildableStructureSocket socket)
        {
            return socket != null
                && socket.isActiveAndEnabled
                && socket.Owner != null
                && !socket.IsOccupied;
        }
    }
}


