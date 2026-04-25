import { extend } from "@pixi/react";
import { Text, Ticker } from "pixi.js";
import { useEffect, useRef, useState } from "react";

extend({ Text });

type Props = {
  value: number;
  x: number;
  y: number;
  onComplete: () => void;
};

const easeOutCubic = (t: number) => 1 - Math.pow(1 - t, 3);
const easeOutBack = (t: number) => {
  const c1 = 1.70158;
  const c3 = c1 + 1;

  return 1 + c3 * Math.pow(t - 1, 3) + c1 * Math.pow(t - 1, 2);
};

export function DamageText({ value, x, y, onComplete }: Props) {
  const [state, setState] = useState({
    offsetX: 0,
    offsetY: 0,
    alpha: 1,
    scale: 1,
  });

  const elapsedRef = useRef(0);
  const startOffsetXRef = useRef((Math.random() - 0.5) * 18);

  useEffect(() => {
    const duration = 1000;
    const ticker = Ticker.shared;

    const update = (ticker: Ticker) => {
      elapsedRef.current += ticker.deltaMS;

      const t = Math.min(1, elapsedRef.current / duration);

      const moveT = easeOutCubic(t);
      const popT = Math.min(1, t / 0.28);
      const fadeT = Math.max(0, (t - 0.45) / 0.55);

      setState({
        offsetX: startOffsetXRef.current * moveT,
        offsetY: -48 * moveT,
        alpha: 1 - easeOutCubic(fadeT),
        scale: 0.65 + easeOutBack(popT) * 0.55,
      });

      if (t >= 1) {
        ticker.remove(update);
        onComplete();
      }
    };

    ticker.add(update);

    return () => {
      ticker.remove(update);
    };
  }, [onComplete]);

  return (
    <pixiText
      text={`-${value}`}
      x={x + state.offsetX}
      y={y + state.offsetY}
      anchor={{ x: 0.5, y: 0.5 }}
      alpha={state.alpha}
      scale={state.scale}
      style={{
        fill: "#ff4d4f",
        fontSize: 22,
        fontWeight: "800",
        stroke: {
          color: "#2b0000",
          width: 5,
        },
        dropShadow: {
          color: "#000000",
          blur: 5,
          distance: 2,
          alpha: 0.7,
        },
      }}
    />
  );
}