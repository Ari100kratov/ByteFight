using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Abstractions.Messaging;

namespace Application.Game.Characters.Create;

public record CreateCharacterCommand(string Name) : ICommand<Guid>;
