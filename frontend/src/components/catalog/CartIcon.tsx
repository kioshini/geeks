import React from 'react';
import { ShoppingCart } from 'lucide-react';
import type { CartIconProps } from '../../types/catalog';

/**
 * Компонент иконки корзины с счетчиком товаров
 * 
 * @param props - Пропсы компонента
 * @returns JSX элемент иконки корзины
 */
export function CartIcon({ totalItems }: CartIconProps) {
  return (
    <div className="flex items-center gap-2">
      {/* Иконка корзины */}
      <ShoppingCart className="w-5 h-5 text-current" />
      
      {/* Счетчик товаров справа от текста */}
      {totalItems > 0 && (
        <span className="bg-orange-500 text-white text-xs font-bold rounded-full h-5 w-5 flex items-center justify-center min-w-[20px]">
          {totalItems > 99 ? '99+' : totalItems}
        </span>
      )}
    </div>
  );
}
