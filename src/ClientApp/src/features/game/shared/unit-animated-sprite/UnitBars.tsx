import { extend } from "@pixi/react";
import { Graphics, Container } from "pixi.js";
import type { UnitRuntime } from "../../types/UnitRuntime";

extend({ Graphics, Container });

interface Props {
  unit: UnitRuntime;
  x: number;
  y: number;
  spriteHeight: number;
  cellWidth: number;
}

export function UnitBars({ unit, x, y, spriteHeight, cellWidth }: Props) {
  const barWidth = cellWidth - 12;
  const barHeight = 5;
  const barYOffset = -5; // смещение сверху

  const hpRatio = unit.hp.current / unit.hp.max;
  const mpRatio = unit.mp ? unit.mp.current / unit.mp.max : 0;

  // бар рисуем чуть выше спрайта
  const hpY = y - spriteHeight * 0.8 - barYOffset;
  const mpY = hpY + barHeight + 2;
  const barX = x - barWidth / 2

  return (
    <pixiContainer>
      {/* HP */}
      <pixiGraphics
        draw={(g) => {
          g.clear();
          g.fill({ color: 0x000000, alpha: 0.5 }).rect(barX, hpY, barWidth, barHeight);
          g.fill({ color: 0xff3b3b }).rect(barX, hpY, barWidth * hpRatio, barHeight);
        }}
      />

      {/* MP */}
      {unit.mp && (
        <pixiGraphics
          draw={(g) => {
            g.clear();
            g.fill({ color: 0x000000, alpha: 0.5 }).rect(barX, mpY, barWidth, barHeight);
            g.fill({ color: 0x4a90e2 }).rect(barX, mpY, barWidth * mpRatio, barHeight);
          }}
        />
      )}
    </pixiContainer>
  );
}
