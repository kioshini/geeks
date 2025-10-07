import { createContext } from 'react';
import type { Unit } from '../types/units';

interface UnitContextType {
  selectedUnit: Unit;
  setSelectedUnit: (unit: Unit) => void;
}

export const UnitContext = createContext<UnitContextType | undefined>(undefined);
