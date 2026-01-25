import { extend } from "@pixi/react";
import { Graphics, Container } from "pixi.js";
import type { UnitRuntime } from "../../types/UnitRuntime";

extend({ Graphics, Container });

interface Props {
  runtime: UnitRuntime;
  x: number;
  y: number;
  spriteHeight: number;
  cellWidth: number;
}

export function UnitBars({ runtime, x, y, spriteHeight, cellWidth }: Props) {
  const barWidth = cellWidth - 12;
  const barHeight = 5;
  const barYOffset = -5; // смещение сверху

  const hpRatio = runtime.hp.current / runtime.hp.max;
  const mpRatio = runtime.mp ? runtime.mp.current / runtime.mp.max : 0;

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
          g.rect(barX, hpY, barWidth, barHeight).fill({ color: 0x000000, alpha: 0.5 });
          g.rect(barX, hpY, barWidth * hpRatio, barHeight).fill({ color: 0xff3b3b });
        }}
      />

      {/* MP */}
      {runtime.mp && (
        <pixiGraphics
          draw={(g) => {
            g.clear();
            g.rect(barX, mpY, barWidth, barHeight).fill({ color: 0x000000, alpha: 0.5 });
            g.rect(barX, mpY, barWidth * mpRatio, barHeight).fill({ color: 0x4a90e2 });
          }}
        />
      )}
    </pixiContainer>
  );
}
