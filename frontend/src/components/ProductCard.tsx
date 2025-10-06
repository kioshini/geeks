import type { ProductDto } from '../lib/api';
import { useUnit } from '../contexts/UnitContext';

/**
 * Props for ProductCard component
 */
interface ProductCardProps {
  /** Product data to display */
  product: ProductDto;
  /** Callback function when add button is clicked */
  onAdd: () => void;
}

/**
 * ProductCard component displays a single product with discount information
 * 
 * @param props - Component props
 * @returns JSX element representing a product card
 * 
 * @example
 * ```tsx
 * <ProductCard 
 *   product={productData} 
 *   onAdd={() => addToCart(product.id)} 
 * />
 * ```
 */
export function ProductCard({ product, onAdd }: ProductCardProps) {
  const { selectedUnit } = useUnit();
	return (
		<div className="bg-white rounded-lg border border-gray-200 p-4 hover:shadow-md transition-shadow">
			<div className="flex items-start justify-between mb-3">
				<div className="flex-1 min-w-0">
					<h3 className="text-lg font-medium text-gray-900 mb-2 line-clamp-2">
						{product.name}
					</h3>
					<div className="flex items-center gap-2 text-sm text-gray-500">
						<span className="px-2 py-1 bg-gray-100 rounded text-xs">
							{product.material}
						</span>
						{product.diameter && (
							<span className="px-2 py-1 bg-gray-100 rounded text-xs">
								Ø{product.diameter}мм
							</span>
						)}
						{product.thickness && (
							<span className="px-2 py-1 bg-gray-100 rounded text-xs">
								{product.thickness}мм
							</span>
						)}
					</div>
				</div>

				{/* Stock Status */}
				<div className={`px-2 py-1 rounded text-xs font-medium ${
					product.isAvailable
						? 'bg-green-100 text-green-800'
						: 'bg-red-100 text-red-800'
				}`}>
					{product.isAvailable ? 'В наличии' : 'Нет в наличии'}
				</div>
			</div>

			{product.description && (
				<p className="text-sm text-gray-600 mb-3 line-clamp-2">
					{product.description}
				</p>
			)}

			<div className="flex items-center justify-between">
				<div>
					{/* Price with discount */}
					<div className="flex items-center gap-2">
						{product.finalPrice && product.finalPrice < product.price ? (
							<>
								<div className="text-xl font-semibold text-green-600">
									{product.finalPrice.toFixed(2)} ₽
								</div>
								<div className="text-lg text-gray-400 line-through">
									{product.price.toFixed(2)} ₽
								</div>
								{product.totalDiscountPercent && product.totalDiscountPercent > 0 && (
									<div className="px-2 py-1 bg-green-100 text-green-800 text-xs font-medium rounded">
										-{product.totalDiscountPercent.toFixed(0)}%
									</div>
								)}
							</>
						) : (
							<div className="text-xl font-semibold text-gray-900">
								{product.price.toFixed(2)} ₽
							</div>
						)}
					</div>
					<div className="text-sm text-gray-500">
						за {selectedUnit}
					</div>
					
					{/* Discount info */}
					{(product.hasVolumeDiscount || product.hasMaterialDiscount) && (
						<div className="text-xs text-gray-500 mt-1">
							{product.hasVolumeDiscount && product.volumeDiscountThreshold && (
								<div>Скидка {product.volumeDiscountPercent}% от {product.volumeDiscountThreshold}м</div>
							)}
							{product.hasMaterialDiscount && product.materialDiscountPercent && (
								<div>Скидка {product.materialDiscountPercent}% за материал</div>
							)}
						</div>
					)}
				</div>
				
				<button 
					onClick={onAdd} 
					disabled={!product.isAvailable} 
					className="px-4 py-2 bg-orange-500 text-white text-sm font-medium rounded-md hover:bg-orange-600 disabled:opacity-50 disabled:cursor-not-allowed transition-colors"
				>
					Добавить
				</button>
			</div>
		</div>
	);
}
