// import { useState } from 'react';

/** Available units of measurement */
export type Unit = 'м' | 'т';

/**
 * Props for UnitToggle component
 */
interface UnitToggleProps {
  /** Currently selected unit */
  selectedUnit: Unit;
  /** Callback when unit changes */
  onUnitChange: (unit: Unit) => void;
  /** Additional CSS classes */
  className?: string;
}

/**
 * UnitToggle component provides a toggle interface for selecting units of measurement
 * 
 * @param props - Component props
 * @returns JSX element representing unit toggle buttons
 * 
 * @example
 * ```tsx
 * <UnitToggle 
 *   selectedUnit="м" 
 *   onUnitChange={setUnit} 
 *   className="w-64" 
 * />
 * ```
 */
export function UnitToggle({ selectedUnit, onUnitChange, className = '' }: UnitToggleProps) {
  const units: { value: Unit; label: string; description: string }[] = [
    { value: 'м', label: 'Метры', description: 'Погонные метры' },
    { value: 'т', label: 'Тонны', description: 'Вес в тоннах' }
  ];

  const handleUnitChange = (unit: Unit) => {
    console.log('Unit changed to:', unit);
    onUnitChange(unit);
  };

  return (
    <div className={`flex bg-gray-100 rounded-lg p-1 ${className}`}>
      {units.map((unit) => (
        <button
          key={unit.value}
          onClick={() => handleUnitChange(unit.value)}
          className={`flex-1 px-3 py-2 text-sm font-medium rounded-md transition-colors ${
            selectedUnit === unit.value
              ? 'bg-white text-orange-600 shadow-sm'
              : 'text-gray-600 hover:text-gray-900'
          }`}
          title={unit.description}
        >
          {unit.label}
        </button>
      ))}
    </div>
  );
}
