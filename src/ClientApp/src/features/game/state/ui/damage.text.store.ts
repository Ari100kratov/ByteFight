import { create } from "zustand";

type DamageText = {
  id: string;
  unitId: string;
  value: number;
};

type DamageTextStore = {
  items: DamageText[];
  add: (unitId: string, value: number) => void;
  remove: (id: string) => void;
  reset: () => void;
};

export const useDamageTextStore = create<DamageTextStore>((set) => ({
  items: [],

  add: (unitId, value) =>
    set((s) => ({
      items: [
        ...s.items,
        {
          id: crypto.randomUUID(),
          unitId,
          value,
        },
      ],
    })),

  remove: (id) =>
    set((s) => ({
      items: s.items.filter((x) => x.id !== id),
    })),

  reset: () => set({ items: [] }),
}));