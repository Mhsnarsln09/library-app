import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { TestPageComponent } from './test-page.component';
import { TEST_FEATURE_CONFIG } from './test-feature.config';
import { TestFacadeService } from './test-facade.service';

const routes: Routes = [
  {
    path: '',
    component: TestPageComponent
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes), TestPageComponent],
  providers: [
    TestFacadeService,
    {
      provide: TEST_FEATURE_CONFIG,
      useValue: {
        title: 'Test Module',
        description: 'Ayrı bir lazy module altında mevcut componentleri tekrar kullanan örnek alan.',
        pageSizeOptions: [4, 8, 12]
      }
    }
  ]
})
export class TestModule {}
